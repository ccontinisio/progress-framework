using System;
using UnityEditor;
using UnityEngine;

namespace ProgressFramework.Save
{
	/// <summary>
	/// <para>Generic implementation of a "game save", providing the ability to save,
	/// load and delete game state through one or more Snapshots (<see cref="SnapshotBase"/>).</para>
	/// 
	/// <para>Supports the concept of "slots", to allow the storage of multiple saves per game.
	/// Slots can be fixed (i.e. Slot A, Slot B, Slot C), or they could be tied to a dynamic key,
	/// for instance the date, to allow creation of endless slots.
	/// The slot keys themselves can be stored in a global Snapshot of its own.</para>
	/// </summary>
    public abstract class SaveBase : ScriptableObject
    {
		[Header("File names")]
		[SerializeField] protected string saveFilename = "save.prgrs";
		[SerializeField] protected string backupSaveFilename = "save.prgrs.bak";

		[Header("Debug")]
		[SerializeField] protected bool _logEvents = false;

        public abstract void NewGame();
		public abstract void DeleteSaveGame(string slotKey);

		/// <summary>
		/// Loads <see cref="SnapshotBase"/> values from a file.
		/// </summary>
		/// <param name="snapshotKey">An identifier that defines which Snapshot to load.</param>
		/// <param name="slotKey">An identifier of the slot to load, if the game supports multiple slots.</param>
		/// <param name="snapshotToLoadInto">The ScriptableObject that will host the values loaded from disk.</param>
		/// <param name="success">Whether the load was completed successfully.</param>
		/// <typeparam name="T">A ScriptableObject deriving from <see cref="SnapshotBase"/></typeparam>
		public void LoadSnapshot<T>(string snapshotKey, string slotKey, T snapshotToLoadInto, out bool success) where T : SnapshotBase
		{
			string path = GetFilePath(snapshotKey, slotKey);

			if (FileManager.LoadFromFile(path, out var json))
			{
				if (_logEvents) Debug.Log("Loaded JSON: " + json);

				if (!string.IsNullOrEmpty(json))
				{
					snapshotToLoadInto.LoadFromJson(json);

					if (_logEvents) Debug.Log("Snapshot loaded successfully from save file.");
					success = true;
				}
				else
				{
					Debug.LogWarning("The save file is empty at " + path);
					success = false;
				}
			}
			else
			{
				success = false;
			}
		}

		/// <summary>
		/// Writes all the values of a <see cref="SnapshotBase"/> to disk.
		/// </summary>
		/// <param name="snapshotToSave">The ScriptableObject that needs to be saved to disk.</param>
		/// <param name="snapshotKey">An identifier that defines which Snapshot is being saved.</param>
		/// <param name="slotKey">An identifier of the slot to save to, if the game supports multiple slots.</param>
		/// <typeparam name="T">A ScriptableObject deriving from <see cref="SnapshotBase"/></typeparam>
		public void SaveSnapshot<T>(T snapshotToSave, string snapshotKey, string slotKey) where T : SnapshotBase
		{
			string path = GetFilePath(snapshotKey, slotKey);
			string backupPath = GetBackupFilePath(snapshotKey, slotKey);
			
			//Backup old file
			if (!FileManager.MoveFile(path, backupPath))
				Debug.LogWarning("Couldn't create a backup save file at " + backupPath);
			
			//Write new file
			string fileContents = snapshotToSave.ToJson();
			if (FileManager.WriteToFile(path, fileContents))
			{
				if(_logEvents) Debug.Log("Generated JSON: " + fileContents);
				if(_logEvents) Debug.Log("Save successful: " + path);
			}
			else
			{
				Debug.LogError("Failed to write save to file: " + path);
			}
		}

		/// <summary>
		/// Deletes the content of a file on disk.
		/// Next time that a Snapshot is loaded from the file, loading will fail.
		/// </summary>
		/// <param name="snapshotKey">An identifier that defines which Snapshot is being deleted.</param>
		/// <param name="slotKey">An identifier of the slot the Snapshot belongs to, if the game supports multiple slots.</param>
		public void DeleteSnapshot(string snapshotKey, string slotKey)
		{
			string path = GetFilePath(snapshotKey, slotKey);
			if (FileManager.WriteToFile(path, ""))
			{
				if(_logEvents) Debug.Log("Save data file deleted at " + path);
			}
			else
			{
				Debug.LogError("Failed to delete save file on disk: " + path);
			}
		}
		
#if UNITY_EDITOR
	    /// <summary>
	    /// Clones a Snapshot into a new file.
	    /// Useful for dumping a set of values tweaked during runtime, that one wants to reuse later.
	    /// </summary>
	    /// <param name="snapshot">The Snapshot to clone.</param>
	    public void SaveSnapshotToScriptableObject(SnapshotBase snapshot)
	    {
		    string filePath = EditorUtility.SaveFilePanelInProject("Save FullGameState", "SavedSnapshot", "asset", "");
		    if (!string.IsNullOrEmpty(filePath))
		    {
			    SnapshotBase storedGameState = Instantiate(snapshot);
			    //TODO: Make sure all variables are References set to "Set Here"
			    AssetDatabase.CreateAsset(storedGameState, filePath);

			    if (_logEvents) Debug.Log("GameState dumped to " + filePath);
		    }
	    }
#endif
	    
	    private string GetFilePath(string snapshotKey, string slotKey) => snapshotKey + "_" + slotKey + "_" + saveFilename;
	    private string GetBackupFilePath(string snapshotKey, string slotKey) => snapshotKey + "_" + slotKey + "_" + backupSaveFilename;

	    [Serializable]
		protected class SnapshotStruct
		{
			public SnapshotBase current;
			public SnapshotBase initial;
		}
    }
}
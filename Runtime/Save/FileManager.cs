﻿using System;
using System.IO;
using UnityEngine;

namespace ProgressFramework.Save
{
	public static class FileManager
	{
		public static bool WriteToFile(string fileName, string fileContents)
		{
			string fullPath = Path.Combine(Application.persistentDataPath, fileName);

			try
			{
				File.WriteAllText(fullPath, fileContents);
				return true;
			}
			catch (Exception e)
			{
				Debug.LogError($"Failed to write to {fullPath} with exception {e}");
				return false;
			}
		}

		public static bool LoadFromFile(string fileName, out string result)
		{
			string fullPath = Path.Combine(Application.persistentDataPath, fileName);
			if(!File.Exists(fullPath))
			{
				File.WriteAllText(fullPath, ""); 
			}
			try
			{
				result = File.ReadAllText(fullPath);
				return true;
			}
			catch (Exception e)
			{
				Debug.LogError($"Failed to read from {fullPath} with exception {e}");
				result = "";
				return false;
			}
		}

		public static bool MoveFile(string fileName, string newFileName)
		{
			string fullPath = Path.Combine(Application.persistentDataPath, fileName);
			string newFullPath = Path.Combine(Application.persistentDataPath, newFileName);

			try
			{
				if (File.Exists(newFullPath))
				{
					File.Delete(newFullPath);
				}

				if (!File.Exists(fullPath))
				{
					return false;
				}
			
				File.Move(fullPath, newFullPath);
			}
			catch (Exception e)
			{
				Debug.LogError($"Failed to move file from {fullPath} to {newFullPath} with exception {e}");
				return false;
			}

			return true;
		}
	}
}

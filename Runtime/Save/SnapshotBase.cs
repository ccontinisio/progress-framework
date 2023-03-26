using UnityEngine;

namespace ProgressFramework.Save
{
    /// <summary>
    /// <para>A snapshot is a fragment of a game save (<see cref="SaveBase"/>), that can be made of one or more values.
    /// Grouping values into a Snapshot allows to keep together the ones that need to be saved or loaded at the same time.
    /// Games then need to implement game-specific Snapshots by inheriting from this class.</para>
    /// 
    /// <para>Note: It is technically possible to hold the whole game state in one Snapshot for simplicity.
    /// However, breaking a game save into multiple Snapshots allows to save states more efficiently:
    /// saving can be broken into multiple operations, distributed in time;
    /// and it also avoids writing too much data that hasn't changed in order to save just a few values.</para>
    /// </summary>
    public abstract class SnapshotBase : ScriptableObject
    {
        public abstract string ToJson();
        public abstract void LoadFromJson(string loadedJson);
        public abstract void TransferValuesFrom(SnapshotBase otherSnapshot);
        //public abstract void ResetToDefault(); //TODO implement in sub-classes
    }
}
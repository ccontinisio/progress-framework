using ProgressFramework.Utilities;
using UnityEngine;

namespace ProgressFramework.Quests
{
    public class Subquest : Quest
    {
        [SeparatorHeader("Development")]
        [InfoBox("Use the Update Names button on the MainQuest asset to rename this asset.", 2)]
        [Tooltip("Use it to provide and end part to the asset name, so it's more recognisable in Object picker and Inspectors. " +
                 "This string is added at the end of the regular sub-asset name, so keep it short.")]
        public string devNameSuffix;
    }
}
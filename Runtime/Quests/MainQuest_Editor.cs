#if UNITY_EDITOR
using System;
using System.Text;
using UnityEditor;

namespace ProgressFramework.Quests
{
    public partial class MainQuest
    {
        private int _cachedSubquestsCount = -1;
        
        protected override void Awake()
        {
            base.Awake();
            CacheTasksCount();
            FixMissingParent();
        }

        private void CacheTasksCount()
        {
            if (_subquests == null)
                _cachedSubquestsCount = 0;
            else
                _cachedSubquestsCount = _subquests.Count;
        }

        protected void OnValidate()
        {
            // TODO Store GUID??
            //base.OnValidate(); //stores GUID
            
            if (QuestsUnlocked != null)
            {
                //Make sure all child Quest SOs reference this as their parent
                foreach (MainQuest q in QuestsUnlocked)
                {
                    if (q != null)
                        q.parentQuest = this;
                }
            }
            
            // TODO: also the activated ones?

            if (_subquests != null
                && _cachedSubquestsCount > -1
                && _cachedSubquestsCount != _subquests.Count) HandleTaskSubAssets();

            FixMissingParent();
        }

        /// <summary>
        /// Manages Task SO subAsset of a Quest SO, when the length of the <see cref="_subquests"/> list has changed,
        /// adding or removing them as needed.
        /// </summary>
        private void HandleTaskSubAssets()
        {
            if (_cachedSubquestsCount < _subquests.Count)
            {
                //A quest has been added
                AddSubquest();
            }
            else if (_cachedSubquestsCount > _subquests.Count)
            {
                //A quest has been removed
                RemoveSubquest();
            }
        }

        /// <summary>
        /// Adds one Subquest to the <see cref="_subquests"/> list and as a sub-asset.
        /// This method is invoked by an Inspector button.
        /// </summary>
        private void AddSubquestFromButton()
        {
            _subquests.Add(null);
            AddSubquest();
        }

        private void AddSubquest()
        {
            string path = AssetDatabase.GetAssetPath(this);
            Subquest subquest = CreateInstance<Subquest>();
            subquest.name = this.name + "_" + _cachedSubquestsCount;
            AssetDatabase.AddObjectToAsset(subquest, path);
            _subquests[_cachedSubquestsCount] = subquest;

            _cachedSubquestsCount = _subquests.Count;
            SaveAndRefreshDB();
        }

        private void RemoveSubquest()
        {
            string path = AssetDatabase.GetAssetPath(this);
            var subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
            foreach (Object subAsset in subAssets)
            {
                Subquest subquestSubAsset = (Subquest) subAsset;
                if (!_subquests.Contains(subquestSubAsset))
                {
                    DestroyImmediate(subquestSubAsset, true);
                    break;
                }
            }

            _cachedSubquestsCount = _subquests.Count;
            SaveAndRefreshDB();
        }

        /// <summary>
        /// Deletes all sub-assets. This method is invoked by an Inspector button.
        /// </summary>
        private void RemoveAllSubquests()
        {
            if (_subquests == null || _subquests.Count <= 0) return;

            for (int i = 0; i < _subquests.Count; i++)
            {
                DestroyImmediate(_subquests[i], true);
            }

            _subquests.Clear();
            _cachedSubquestsCount = 0;

            SaveAndRefreshDB();
        }

        /// <summary>
        /// Renames all sub-assets of the MainQuest asset, to match the name of the main one.
        /// Useful in case the main asset was renamed.
        /// This method is invoked by an Inspector button.
        /// </summary>
        private void FixSubquestNaming()
        {
            if (_subquests == null || _subquests.Count <= 0) return;

            for (int i = 0; i < _subquests.Count; i++)
            {
                _subquests[i].name = ComposeSubquestName(_subquests[i], i);
            }

            SaveAndRefreshDB();
        }

        private string ComposeSubquestName(Subquest subQuest, int index)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendJoin("_", name, index);
            if (!string.IsNullOrEmpty(subQuest.devNameSuffix))
            {
                sb.Append("_");
                sb.Append(subQuest.devNameSuffix);
            }
            
            return sb.ToString();
        }

        private void SaveAndRefreshDB()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary> Sets ParentQuest to a "real null", useful to fix the case when
        /// the field is showing "Missing" due to the parent Quest SO having been deleted.</summary>
        private void FixMissingParent()
        {
            if (parentQuest == null)
                parentQuest = null;
        }
    }
}
#endif
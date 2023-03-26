using System;
using System.Collections.Generic;
using ProgressFramework.Save;
using ProgressFramework.Utilities;
using UnityEngine;
using QuestState = ProgressFramework.Quests.Quest.QuestState;

namespace ProgressFramework.Quests
{
    [CreateAssetMenu(fileName = "QuestsProgressSnapshot", menuName = "Progress Framework/Quests Snapshot")]
    public class QuestsProgressSnapshot : SnapshotBase
    {
        [SerializeField] private MainQuest _firstQuest;
        [Space(2f)]
        [Button("RefreshQuestList", "Rebuild Progress list")]
        [SerializeField] private bool _dummyField;
        
        [Space(10f)]
        [InfoBox("Don't add/remove Quests to the list below. " +
                 "Provide the First Quest above, and press the button to rebuild.", 3)]
        [SerializeField] private List<QuestProgress> _progress;
        
        [ButtonAttribute(new string[]{"SetAllQuestsToComplete", "SetAllQuestsToLocked"},
            new string[]{"All to complete", "Clear all progress"})]
        [SerializeField] private bool _dummyField2;

        public MainQuest FirstQuest => _firstQuest;
        public List<QuestProgress> Progress => _progress;

        /// <summary>
        /// Rebuilds the <see cref="_progress"/> list from scratch, starting from <see cref="_firstQuest"/> and
        /// iterating through all subsequent unlocked, activated quests, and subquests. Invoked by an Inspector button.
        /// </summary>
        private void RefreshQuestList()
        {
            _progress = new List<QuestProgress>();
            AddQuestAndDependents(_firstQuest);
            _progress[0] = new QuestProgress(_firstQuest, QuestState.Active);
            
            //Recursive search
            void AddQuestAndDependents(MainQuest mainQuest)
            {
	            //Quest is already in the list, it's referenced by multiple parent quests.
	            if (_progress.Exists(x => x.quest == mainQuest))
	            {
		            Debug.LogWarning("The quest " + mainQuest.name + " was found twice while going through Quest connections. " +
		                             "Make sure a Quest is only Activated or Unlocked by one Parent Quest.", this);
		            return;
	            }
				
	            _progress.Add(new QuestProgress(mainQuest));
	            
	            //Loop through all Quests unlocked and activated by this one, plus its Subquests
                foreach (Quest sq in mainQuest.Subquests)
	                _progress.Add(new QuestProgress(sq));
                
                foreach (MainQuest unlockedQuest in mainQuest.QuestsUnlocked)
                    AddQuestAndDependents(unlockedQuest);
                
                foreach (MainQuest childQuest in mainQuest.QuestsActivated)
	                AddQuestAndDependents(childQuest);
            }
        }

        public QuestState GetQuestState(Quest quest)
        {
	        return _progress.Find(x => x.quest == quest).state;
        }

        public void MarkQuestAs(Quest whichQuest, QuestState newState)
		{
			int index = _progress.FindIndex(x => x.quest == whichQuest);
			//TODO: this breaks when the QuestManager is iterating during ReloadQuestStates, because the collection changes
			QuestProgress qp = _progress[index] = new QuestProgress(whichQuest, newState);
		}

		public override string ToJson()
		{
			//TODO: Fix SO saving: need GUID, not direct save
			ProgressArrayStruct pas = new ProgressArrayStruct();
			pas.progressArray = _progress;
			return JsonUtility.ToJson(pas);
		}

		public override void LoadFromJson(string loadedJson)
		{
			//TODO: Fix SO loading: need GUID, not direct load
			ProgressArrayStruct pas = JsonUtility.FromJson<ProgressArrayStruct>(loadedJson);
			_progress = pas.progressArray;
		}

		public override void TransferValuesFrom(SnapshotBase otherSnapshot)
		{
			QuestsProgressSnapshot qs = (QuestsProgressSnapshot)otherSnapshot;

			_firstQuest = qs.FirstQuest;

			_progress = new List<QuestProgress>(); //reset length
			for (int i = 0; i < qs.Progress.Count; i++)
			{
				_progress.Add(qs.Progress[i]);
			}
		}

#if UNITY_EDITOR
	    /// <summary>
	    /// Utility method to set all quest progress to a specific value. Called by buttons in the Inspector.
	    /// </summary>
	    private void SetAllQuestsTo(QuestState newState)
	    {
		    QuestProgress questProgress;
		    for(int i=0; i<_progress.Count; i++)
		    {
			    questProgress = _progress[i];
			    questProgress.state = newState;
			    _progress[i] = questProgress;
		    }
	    }
	    private void SetAllQuestsToComplete() => SetAllQuestsTo(QuestState.Completed);
	    private void SetAllQuestsToLocked() => SetAllQuestsTo(QuestState.Locked);
#endif
	}

    [Serializable]
    public struct ProgressArrayStruct
    {
	    public List<QuestProgress> progressArray;
    }

    [Serializable]
    public struct QuestProgress
    {
        public Quest quest;
        public QuestState state;

        public QuestProgress(Quest q)
        {
            quest = q;
            state = QuestState.Locked;
        }

		public QuestProgress(Quest q, QuestState state)
		{
			quest = q;
			this.state = state;
		}
    }
}
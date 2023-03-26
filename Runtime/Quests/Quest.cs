using System.Collections.Generic;
using ProgressFramework.Utilities;
using SOTools;
using SOTools.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace ProgressFramework.Quests
{
    public class Quest : ManagedScriptableObject
    {
        [ReadOnly, ShowInPlayMode] public QuestState state = QuestState.Locked;
        [ReadOnly, ShowInPlayMode] public List<GameObject> listeningActions = new List<GameObject>();

        [field: SeparatorHeader("Player facing info")]
        [field: SerializeField] public string Title { private set; get; }
        [field: SerializeField] public string Description { private set; get; }

        public event UnityAction<Quest, bool> Unlocked;
        public event UnityAction<Quest, bool> Activated;
        public event UnityAction<Quest, bool> Completed;
        public event UnityAction<Quest, bool> Failed;

        public void Unlock(bool fastForwarded)
        {
            state = QuestState.Available;
            
            Unlocked?.Invoke(this, fastForwarded);
        }

        public virtual void Activate(bool fastForwarded)
        {
            state = QuestState.Active;

            Activated?.Invoke(this, fastForwarded);
        }

        public virtual void Complete(bool fastForwarded)
        {
            state = QuestState.Completed;

            Completed?.Invoke(this, fastForwarded);
        }

        public void Fail(bool fastForwarded)
        {
            state = QuestState.Failed;
            
            Failed?.Invoke(this, fastForwarded);
        }

        protected override void Reset()
        {
            state = QuestState.Locked;
            listeningActions?.Clear();
        }

        public enum QuestState
        {
            [Tooltip("Default state. The Quest is not accessible and depends on a previous quest to unlock.")] Locked,
            [Tooltip("Has been unlocked by a previous quest, but hasn't started yet.")] Available,
            [Tooltip("Is active and can be completed.")] Active,
            [Tooltip("Has been successfully completed.")] Completed,
            [Tooltip("Was permanently failed.")] Failed,
        }
    }
}
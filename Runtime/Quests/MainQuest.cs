using System.Collections.Generic;
using ProgressFramework.Utilities;
using UnityEngine;
using UnityEngine.Events;
using ReadOnlyAttribute = SOTools.Utilities.ReadOnlyAttribute;

namespace ProgressFramework.Quests
{
	[CreateAssetMenu(menuName = "Progress Framework/Main Quest", fileName = "00X_QuestTitle")]
	public partial class MainQuest : Quest
	{
		[ReadOnly, Tooltip("The quest that unlocks this one.")] public MainQuest parentQuest;

		[SeparatorHeader("Subquests")]
		[Button(new string[]{"AddSubquestFromButton", "RemoveAllSubquests"}, 
			new string[]{"(+) Subquest", "Remove all"})]
		[SerializeField] private bool _dummyField;
		
		[Button("FixSubquestNaming", "Update names")]
		[SerializeField] private bool _dummyField2;
		
		[InfoBox("Use the buttons above to add/remove Subquests. The +/- buttons also work, and elements can be rearranged. Don't modify the array length directly.", 3)]
		[SerializeField, ReadOnly] private List<Subquest> _subquests = new List<Subquest>();
		
		[field:SeparatorHeader("Once completed")]
		[field: SerializeField,
		        Tooltip("Quest(s) unlocked by this one. They will be available to be activated (i.e. by a dialogue), but won't be active just yet.")]
		public MainQuest[] QuestsUnlocked { private set; get; }

		[field: SerializeField,
		        Tooltip("Quest(s) activated by this one. Is usually empty for side quests.")]
		public MainQuest[] QuestsActivated { private set; get; }
		
		public List<Subquest> Subquests => _subquests;

		private int _currentSubquestIndex = 0;

		public event UnityAction<Subquest, QuestState> SubquestStateChanged;

		/// <inheritdoc cref="Quest.Activate"/>
		public override void Activate(bool fastForwarded)
		{
			base.Activate(fastForwarded);

			_currentSubquestIndex = 0;
			
			if (_subquests.Count != 0)
			{
				Subquest sq = _subquests[_currentSubquestIndex];
				sq.Completed += OnSubquestCompleted;
				sq.Activate(fastForwarded);
				
				SubquestStateChanged?.Invoke(sq, QuestState.Active);
			}
		}

		/// <summary>
		/// <para>Event handler that fires when a Subquest of this MainQuest becomes complete. If it's the last Subquest, the whole MainQuest becomes complete.</para>
		/// <para><see cref="QuestManager"/> doesn't manage Subquests, but is notified of their state change to write it in the progress snapshot.</para>
		/// </summary>
		private void OnSubquestCompleted(Quest subquest, bool fastForwarded)
		{
			Subquest sq;
			if(_currentSubquestIndex + 1 < _subquests.Count)
			{
				//There are more Subquests to complete
				sq = _subquests[_currentSubquestIndex];
				sq.Completed -= OnSubquestCompleted;
				SubquestStateChanged?.Invoke(sq, QuestState.Completed);
				
				_currentSubquestIndex++;
				
				sq = _subquests[_currentSubquestIndex];
				sq.Completed += OnSubquestCompleted;
				sq.Activate(fastForwarded);
				SubquestStateChanged?.Invoke(sq, QuestState.Active);
			}
			else
			{
				//No more Subquests, this Main Quest is complete
				sq = _subquests[_currentSubquestIndex];
				sq.Completed -= OnSubquestCompleted;
				SubquestStateChanged?.Invoke(sq, QuestState.Completed);

				base.Complete(fastForwarded);
			}
		}

		/// <inheritdoc cref="Quest.Complete"/>
		public override void Complete(bool fastForwarded)
		{
			//Will force complete all subquests that are not completed yet
			foreach (Subquest sq in _subquests)
			{
				if (sq.state != QuestState.Completed)
				{
					sq.Completed -= OnSubquestCompleted;
					sq.Complete(true);
				}
			}
			
			base.Complete(fastForwarded);
		}

		protected override void Reset()
		{
			base.Reset();
			_currentSubquestIndex = 0; 
		}
	}
}

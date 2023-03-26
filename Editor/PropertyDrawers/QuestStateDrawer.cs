using ProgressFramework.Quests;
using UnityEditor;
using UnityEngine;

namespace ProgressFramework.Editor.Editors
{
    [CustomPropertyDrawer(typeof(Quest.QuestState))]
    public class QuestStateDrawer : PropertyDrawer
    {
        private float _gap = 4f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float originalWidth = position.width;
            
            GUIContent content = GUIContent.none;
            switch (property.intValue)
            {
                case (int)Quest.QuestState.Locked:
                    content = EditorGUIUtility.IconContent("TestIgnored");
                    break;
                case (int)Quest.QuestState.Available:
                    content = EditorGUIUtility.IconContent("TestNormal");
                    break;
                case (int)Quest.QuestState.Active:
                    content = EditorGUIUtility.IconContent("DotFill");
                    break;
                case (int)Quest.QuestState.Completed:
                    content = EditorGUIUtility.IconContent("TestPassed");
                    break;
                case (int)Quest.QuestState.Failed:
                    content = EditorGUIUtility.IconContent("TestFailed");
                    break;
            }
            
            
            if(label != GUIContent.none)
            {
                EditorGUI.PrefixLabel(position, label);
                position.x += EditorGUIUtility.labelWidth;
                position.width -= EditorGUIUtility.labelWidth;
            }

            position.width = position.height;
            position.height -= 4;
            EditorGUI.LabelField(position, content);

            position.height += 4;
            position.x += position.width + _gap;
            position.width = originalWidth - position.height - _gap;
            EditorGUI.PropertyField(position, property, GUIContent.none);
        }
    }
}
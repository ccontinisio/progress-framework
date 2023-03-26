using ProgressFramework.Quests;
using UnityEditor;
using UnityEngine;

namespace ProgressFramework.Editor.Editors
{
    [CustomPropertyDrawer(typeof(QuestProgress), false)]
    public class QuestProgressDrawer : PropertyDrawer
    {
        private float dropdownWidth = 94f;
        private float padding = 4f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height -= padding;
            
            //Draw disabled Quest picker
            SerializedProperty questProp = property.FindPropertyRelative("quest");
            if (questProp.objectReferenceValue as Subquest != null)
            {
                //Add indent
                position.x += 10;
                position.width -= 10;
            }
                
            position.width -= dropdownWidth;
            GUI.enabled = false;
            EditorGUI.PropertyField(position, questProp, GUIContent.none);
            GUI.enabled = true;

            //Draw QuestState dropdown
            position.x += position.width + padding;
            position.width = dropdownWidth - padding;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("state"), GUIContent.none);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + padding;
        }
    }
}
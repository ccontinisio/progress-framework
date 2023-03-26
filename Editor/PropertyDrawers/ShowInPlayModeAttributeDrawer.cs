using ProgressFramework.Utilities;
using UnityEditor;
using UnityEngine;

namespace ProgressFramework.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(ShowInPlayModeAttribute))]
    public class ShowInPlayModeAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property,
            GUIContent label)
        {
            if (EditorApplication.isPlaying)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorApplication.isPlaying ? base.GetPropertyHeight(property, label) : 0f;
        }
    }
}
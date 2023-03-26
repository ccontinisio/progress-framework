using ProgressFramework.Utilities;
using UnityEditor;
using UnityEngine;

namespace ProgressFramework.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(SeparatorHeaderAttribute))]
    public class SeparatorHeaderAttributeDrawer : DecoratorDrawer
    {
        private SeparatorHeaderAttribute separatorHeaderAttribute => (SeparatorHeaderAttribute)attribute;
        private float leftGutter = 18f;
        private float rightGutter = 4f;
        private float topPadding = 14f;
        private float bottomPadding = 4f;

        public override bool CanCacheInspectorGUI() => true;

        public override float GetHeight() => base.GetHeight() + topPadding + bottomPadding;

        public override void OnGUI(Rect position)
        {
            //Determine colour depending on which editor skin is currently used
            float value = ((EditorGUIUtility.isProSkin) ? .19f : .73f);
            Color colour = new Color(value, value, value, 1f);
            
            float originalWidth = position.width;
            
            //Draw background rectangle, full-width
            position.y += topPadding;
            position.x -= leftGutter;
            position.width += leftGutter + rightGutter;
            position.height -= bottomPadding + topPadding;
            EditorGUI.DrawRect(position, colour);

            //Draw bold label
            position.x += leftGutter;
            position.width = originalWidth;
            EditorGUI.LabelField(position, separatorHeaderAttribute.text, EditorStyles.boldLabel);
        }
    }
}
using ProgressFramework.Utilities;
using UnityEditor;
using UnityEngine;

namespace ProgressFramework.Editor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(InfoBoxAttribute))]
	public class InfoBoxAttributeDrawer : DecoratorDrawer
	{
		private float padding = 4f;
		
		private InfoBoxAttribute InfoBoxAttribute => (InfoBoxAttribute)attribute;

		public override float GetHeight()
		{
			return base.GetHeight() + InfoBoxAttribute.height + (padding * 2f);
		}
		
		public override void OnGUI(Rect position)
		{
			position.y += padding;
			position.height -= padding * 2f;
			EditorGUI.HelpBox(position, InfoBoxAttribute.message, MessageType.Info);
		}
	}
}
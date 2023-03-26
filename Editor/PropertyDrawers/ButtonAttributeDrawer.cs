using System.Reflection;
using ProgressFramework.Utilities;
using UnityEditor;
using UnityEngine;

namespace ProgressFramework.Editor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(ButtonAttribute))]
	public class ButtonAttributeDrawer : PropertyDrawer
	{
		private MethodInfo[] _methodInfo;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 22f;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			ButtonAttribute buttonAttribute = (ButtonAttribute)attribute;
			int numberOfButtons = buttonAttribute.buttonTexts.Length;
			float increment = position.width / numberOfButtons;
			float initialX = position.x;
			position.width /= numberOfButtons;

			//Build the array of methods
			if (_methodInfo == null)
			{
				_methodInfo = new MethodInfo[numberOfButtons];

				for(int i=0; i<numberOfButtons; i++)
				{
					System.Type eventOwnerType = property.serializedObject.targetObject.GetType();
					string eventName = buttonAttribute.methodNames[i];
					_methodInfo[i] = eventOwnerType.GetMethod(eventName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				}
			}
			
			//Draw the buttons
			for(int i=0; i<numberOfButtons; i++)
			{
				position.x = initialX + (i * increment);

				if(GUI.Button(position, buttonAttribute.buttonTexts[i]))
				{
					if (_methodInfo[i] != null)
						_methodInfo[i].Invoke(property.serializedObject.targetObject, null);
					else
						Debug.LogWarning("Method invoked by button not found.");

				}
			}
		}
	}
}
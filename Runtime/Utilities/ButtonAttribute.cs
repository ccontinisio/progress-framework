using System;
using UnityEngine;

namespace ProgressFramework.Utilities
{
	[AttributeUsage(System.AttributeTargets.Field)]
	public class ButtonAttribute : PropertyAttribute
	{
		public readonly string[] methodNames;
		public readonly string[] buttonTexts;

		public ButtonAttribute(string methodToCall, string textToDisplay)
		{
			methodNames = new string[]{methodToCall};
			buttonTexts = new string[]{textToDisplay};
		}

		public ButtonAttribute(string[] methodsToCall, string[] textsToDisplay)
		{
			methodNames = methodsToCall;
			buttonTexts = textsToDisplay;
		}
	}
}

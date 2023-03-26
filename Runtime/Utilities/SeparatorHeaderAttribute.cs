using UnityEngine;

namespace ProgressFramework.Utilities
{
    public class SeparatorHeaderAttribute : PropertyAttribute
    {
        public readonly string text;
        
        public SeparatorHeaderAttribute(string text)
        {
            this.text = text;
        }
    }
}
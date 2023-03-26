using UnityEngine;

namespace ProgressFramework.Utilities
{
    public class InfoBoxAttribute : PropertyAttribute
    {
        public readonly string message;
        public readonly float height;

        public InfoBoxAttribute(string message, int lines = 1)
        {
            this.message = message;
            height = lines * 8f;
        }
    }
}
using UnityEngine;

/// <summary>
/// Maintains various constants.
/// </summary>
public class Constants : MonoBehaviour
{
    public class Tags
    {
        public const string MainCamera = "MainCamera";
    }

    public class Layers
    {
        public static readonly int UI = LayerMask.NameToLayer("UI");
    }    

    public class Input 
	{
		public const string MouseScrollWheel = "Mouse ScrollWheel";
		public const string Horizontal = "Horizontal";
		public const string Vertical = "Vertical";
	    public const string MouseX = "Mouse X";
	    public const string MouseY = "Mouse Y";
	}
}

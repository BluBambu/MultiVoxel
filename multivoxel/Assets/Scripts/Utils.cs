using UnityEngine;

/// <summary>
/// Provides a bunch of utility methods
/// </summary>
public static class Utils
{
    /// <summary>
    /// Returns the given angle clamped between the min and max. The returned angle will always
    /// be between -360 and 360
    /// </summary>
    public static float ClampAngle(float angle, float min, float max)
    {
        while (angle < -360)
        {
            angle += 360;
        }
        while (angle > 360)
        {
            angle -= 360;
        }
        return Mathf.Clamp(angle, min, max);
    }
}

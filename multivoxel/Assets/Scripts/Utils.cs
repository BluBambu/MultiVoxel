using UnityEngine;

/// <summary>
/// Provides a bunch of utility methods
/// </summary>
public static class Utils
{
    /// <summary>
    /// Rounds all of the given vector's components to the nearest integer
    /// </summary>
	public static Vector3 RoundVector3(Vector3 vector3) 
	{
		return new Vector3(Mathf.RoundToInt(vector3.x), Mathf.RoundToInt(vector3.y),
			Mathf.RoundToInt(vector3.z));
	}

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

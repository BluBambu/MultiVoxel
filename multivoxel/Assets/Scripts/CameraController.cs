using UnityEngine;

/// <summary>
/// Dictates the camera movement
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour 
{

    /// <summary>
    /// The delta change in distance the camera is from the world origin when the user scrolls
    /// </summary>
	private const float DeltaDistancePerScroll = .5f;
    private const float InitialDistance = 5.0f;
    private const float MoveSpeedX = 250.0f;
	private const float MoveSpeedY = 120.0f;
	private const float MinAngleY = -90.0f;
	private const float MaxAngleY = 90.0f;
	
	public static CameraController Instance 
	{
		get 
		{
			if (_instance == null) {
				_instance = FindObjectOfType<CameraController>();
			}
			return _instance;
		}
	}
    /// <summary>
    /// Backing field, don't modify
    /// </summary>
	private static CameraController _instance;

    /// <summary>
    /// The distance the camera is from the world origin
    /// </summary>
	public float Distance 
	{
		get {  return _distance; }
		set
		{
			_distance = value;
			UpdateCamera(transform.rotation);
		}
	}
    /// <summary>
    /// Backing field, don't modify
    /// </summary>
	private float _distance;

	private float _deltaX;
	private float _deltaY;

	private void Awake()
	{
		Vector3 angles = transform.eulerAngles;
		Distance = InitialDistance;
		_deltaX = angles.x;
		_deltaY = angles.y;
	}

	private void LateUpdate()
	{
		if (InputManager.CurrentMouseDownType == InputManager.MouseDownType.CameraMove)
		{
			_deltaX += Input.GetAxis(Constants.Input.MouseX) * MoveSpeedX * Time.deltaTime;
			_deltaY -= Input.GetAxis(Constants.Input.MouseY) * MoveSpeedY * Time.deltaTime;
		    _deltaY = Utils.ClampAngle(_deltaY, MinAngleY, MaxAngleY);
			UpdateCamera(Quaternion.Euler(_deltaY, _deltaX, 0));
		}
	}

	public void IncrementDistance() 
	{
		Distance -= DeltaDistancePerScroll;
	}

	public void DecrementDistance() 
	{
		Distance += DeltaDistancePerScroll;
	}

    /// <summary>
    /// Updates the camera to have the given rotation relative to the world origin
    /// </summary>
	private void UpdateCamera(Quaternion rotation)
    {
		Vector3 position = rotation * (new Vector3 (0.0f, 0.0f, -Distance));
		transform.rotation = rotation;
		transform.position = position;
	}
}

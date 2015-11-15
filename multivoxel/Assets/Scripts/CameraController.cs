using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour 
{
    // Change in distance the camera is from the world origin per scroll tick
	private const float DeltaDistancePerScroll = .5f;

    // Initial camera distance from the world origin
    private const float InitialCameraDistance = 5.0f;

    private const float MoveSpeedX = 250.0f;
	private const float MoveSpeedY = 120.0f;
	private const float MinAngleY = -90.0f;
	private const float MaxAngleY = 90.0f;
	
    // Singleton
	public static CameraController Instance 
	{
		get { return _instance ?? (_instance = FindObjectOfType<CameraController>()); }
	}
    // Backing field, don't modify
	private static CameraController _instance;

    // Distance the camera is from the world origin
	private float Distance 
	{
		get {  return _distance; }
		set
		{
			_distance = value;
			Rotation = transform.rotation;
		}
	}
    // Backing field, don't modify
	private float _distance;

    // Rotation of the camera from the world origin
    private Quaternion Rotation
    {
        set
        {
            Vector3 newPos = value * (new Vector3(0f, 0f, -Distance));
            transform.rotation = value;
            transform.position = newPos;
        }
    }

	private float _deltaX;
	private float _deltaY;

	private void Awake()
	{
		Distance = InitialCameraDistance;
		_deltaX = transform.eulerAngles.x;
		_deltaY = transform.eulerAngles.y;
	}

	private void LateUpdate()
	{
		if (InputManager.CurrentMouseActionType == InputManager.MouseActionType.CameraMove)
		{
			_deltaX += Input.GetAxis(Constants.Input.MouseX) * MoveSpeedX * Time.deltaTime;
			_deltaY -= Input.GetAxis(Constants.Input.MouseY) * MoveSpeedY * Time.deltaTime;
		    _deltaY = Utils.ClampAngle(_deltaY, MinAngleY, MaxAngleY);
			Rotation = Quaternion.Euler(_deltaY, _deltaX, 0);
		}
	}

	public void IncrementCamDistance() 
	{
		Distance -= DeltaDistancePerScroll;
	}

	public void DecrementCamDistance() 
	{
		Distance += DeltaDistancePerScroll;
	}
}

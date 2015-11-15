using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
	public enum MouseActionType
	{
		None,
		CameraMove,
		Edit
	}

    // Mouse action type of the left mouse button
	public static MouseActionType CurrentMouseActionType { private set; get; }

    private HSVPicker _hsvColorPicker;
	private VoxelModel _voxelModel;

	private void Awake() 
	{
        _hsvColorPicker = FindObjectOfType<HSVPicker>();
		_voxelModel = FindObjectOfType<VoxelModel>();
	}

	void Update () 
	{
		if (Input.GetMouseButtonDown(0)) {
			RaycastHit hit;
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)) {
                // If we've clicked on the model, then modify the model
				CurrentMouseActionType = MouseActionType.Edit;
				_voxelModel.AddVoxel(ConvertToWorldAdjPos(hit), _hsvColorPicker.currentColor);
			} else {
			    if (!EventSystem.current.IsPointerOverGameObject())
			    {
                    // If we've clicked on nothing (the background), then move the camera
                    CurrentMouseActionType = MouseActionType.CameraMove;
                }
			}
		} else if (Input.GetMouseButtonUp(0)) {
			CurrentMouseActionType = MouseActionType.None;
		}

	    if (Input.GetMouseButtonDown(1))
	    {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                // If we've clicked on the model, then modify the model
                _voxelModel.RemoveVoxel(ConvertToWorldHitPos(hit));
            }
        }

        // TODO: Only zoom when the app has focus
		float scroll = Input.GetAxis(Constants.Input.MouseScrollWheel);
		if (scroll > 0) {
			CameraController.Instance.IncrementCamDistance();
		} else if (scroll < 0) {
			CameraController.Instance.DecrementCamDistance();
		}
	}

    // Gets the closest world position normally adjacent to the raycast hit
    private Vector3 ConvertToWorldAdjPos(RaycastHit hit)
    {
        return new Vector3(MoveAroundBlock(hit.point.x, hit.normal.x, true),
            MoveAroundBlock(hit.point.y, hit.normal.y, true),
            MoveAroundBlock(hit.point.z, hit.normal.z, true));
    }

    // Gets the closest world position that the raycast hits
    private Vector3 ConvertToWorldHitPos(RaycastHit hit)
    {
        return new Vector3(MoveAroundBlock(hit.point.x, hit.normal.x, false),
            MoveAroundBlock(hit.point.y, hit.normal.y, false),
            MoveAroundBlock(hit.point.z, hit.normal.z, false));
    }

    private float MoveAroundBlock(float pos, float norm, bool adjacent)
    {
        if (pos - (int)pos == 0.5f || pos - (int)pos == -0.5f)
        {
            pos += (adjacent ? 1 : -1) * (norm / 2);
        }
        return pos;
    }
}

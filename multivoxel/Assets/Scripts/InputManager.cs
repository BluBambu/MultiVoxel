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
				_voxelModel.AddVoxel(hit, _hsvColorPicker.currentColor);
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

        // TODO: Only zoom when the app has focus
		float scroll = Input.GetAxis(Constants.Input.MouseScrollWheel);
		if (scroll > 0) {
			CameraController.Instance.IncrementCamDistance();
		} else if (scroll < 0) {
			CameraController.Instance.DecrementCamDistance();
		}
	}
}

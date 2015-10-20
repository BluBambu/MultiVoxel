using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Manages all of the user input
/// </summary>
public class InputManager : MonoBehaviour {

    /// <summary>
    /// The type of action that the mouse press/hold/release is
    /// </summary>
	public enum MouseDownType
	{
		None,
		CameraMove,
		Edit
	}

    /// <summary>
    /// This value will be MouseDownType.None if the mouse button isn't held down, otherwise it'll
    /// be the repective mouse action
    /// </summary>
	public static MouseDownType CurrentMouseDownType { private set; get; }

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
				CurrentMouseDownType = MouseDownType.Edit;
				_voxelModel.AddVoxel(hit, _hsvColorPicker.currentColor);
			} else {
			    if (!EventSystem.current.IsPointerOverGameObject())
			    {
                    // If we've clicked on nothing (the background), then move the camera
                    CurrentMouseDownType = MouseDownType.CameraMove;
                }
			}
		} else if (Input.GetMouseButtonUp(0)) {
			CurrentMouseDownType = MouseDownType.None;
		}

        // TODO: Only zoom when the app has focus
        // Zoom the camera in/out if the user scrolls
		float scroll = Input.GetAxis(Constants.Input.MouseScrollWheel);
		if (scroll > 0) {
			CameraController.Instance.IncrementDistance();
		} else if (scroll < 0) {
			CameraController.Instance.DecrementDistance();
		}
	}
}

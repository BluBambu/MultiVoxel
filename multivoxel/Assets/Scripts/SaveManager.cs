using UnityEngine;
using UnityEngine.UI;
using System;

public class SaveManager : MonoBehaviour {
    public Button _saveButton;
    public InputField _filepathInputField;
    private VoxelController _voxelController;

	// Use this for initialization
	void Start () {
        _voxelController = FindObjectOfType<VoxelController>();
        _saveButton.onClick.AddListener(() =>
        {
            try
            {
                print("Button clicked!");
                _voxelController.SaveToFile(_filepathInputField.text);
            }
            catch (Exception e)
            {
                // TODO: print error to user
                Debug.Log("Could not save to '" + _filepathInputField.text + "' due to " + e.ToString());
            }
        });
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

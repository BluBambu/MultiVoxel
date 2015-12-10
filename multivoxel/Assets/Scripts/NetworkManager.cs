using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour
{

	private static bool _hasReceivedModel = false;
    private VoxelController _voxelController;

    private void Awake()
    {
        _voxelController = GameObject.FindObjectOfType<VoxelController>();
    }

	// Update is called once per frame
    private void Update()
    {
        if (_hasReceivedModel)
        {
            // check for command
            VoxelCommand cmd;
            if (Client.TryReceiveTcp<VoxelCommand>(out cmd))
            {
                cmd.Apply(_voxelController);
            }
        }
        else
        {
            // check for model
			SerializedVoxelData data;
            _hasReceivedModel = Client.TryReceiveTcp<SerializedVoxelData>(out data);

            if (_hasReceivedModel)
            {
                // render for first time
				VoxelData voxelData = VoxelSerializer.DeserializeVoxelData(data);
                _voxelController.ChangeData(voxelData);
            }
        }
    }
}

using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour
{

	private static bool _hasReceivedModel = false;
	private static VoxelData _voxelData;
	// Update is called once per frame
    private void Update()
    {
        if (_hasReceivedModel)
        {
            // check for command
            VoxelCommand cmd;
            if (Client.TryReceive<VoxelCommand>(out cmd))
            {
                cmd.Apply(_voxelData);
                VoxelController._voxelRenderer.RenderMesh(_voxelData);
            }
        }
        else
        {
            // check for model
            _hasReceivedModel = Client.TryReceive<VoxelData>(out _voxelData);

            if (_hasReceivedModel)
            {
                // render for first time
                VoxelController._voxelRenderer.RenderMesh(_voxelData);
            }
        }
    }
}

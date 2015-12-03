using UnityEngine;
using System.Collections;

/*
 * This script is a demo of client-side networking.
 * 
 * The Update loop:
 *   checks for the initial model from the server
 *   checks for commands from the server
 *   applies commands to its initial model
 *   renders the model when commands are applied
 * 
 * Usage:
 * 1) Adjust SERVER_PORT in Config.cs as necessary.
 * 2) Build the game as standalone PC app.
 * 3) Run the following shell commands on your local machine:
 *    ./multivoxel.app/Contents/MacOS/multivoxel  # server, client 1
 *    ./multivoxel.app/Contents/MacOS/multivoxel --client-only  # client 2
 *    ./multivoxel.app/Contents/MacOS/multivoxel --client-only  # client 3
 *    ...
 *    ./multivoxel.app/Contents/MacOS/multivoxel --client-only  # client N
 * 4) Add/remove voxels in one window and see corresponding updates in other windows.
 * 
 */
public class Demo : MonoBehaviour {

	private static bool _hasReceivedModel = false;
	private static VoxelData _voxelData;

	// Update is called once per frame
	void Update () {
		if (_hasReceivedModel) {
			// check for command
			VoxelCommand cmd;
			if (Client.TryReceive<VoxelCommand>(out cmd)) {
				cmd.Apply(_voxelData);
				VoxelController._voxelRenderer.RenderMesh(_voxelData);
			}
		} else {
			// check for model
			_hasReceivedModel = Client.TryReceive<VoxelData> (out _voxelData);
			if (_hasReceivedModel) {
				// render for first time
				VoxelController._voxelRenderer.RenderMesh(_voxelData);
			}
		}
	}
}

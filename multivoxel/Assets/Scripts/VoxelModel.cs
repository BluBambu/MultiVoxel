using UnityEngine;

/// <summary>
/// Represents the voxel model in the editor
/// </summary>
[RequireComponent(typeof(VoxelModelRenderer))]
public class VoxelModel : MonoBehaviour 
{
	private VoxelModelRenderer _voxelRenderer;
	private VoxelData _voxelData;

	private void Awake() 
	{
		_voxelRenderer = GetComponent<VoxelModelRenderer>();
		_voxelData = new VoxelData();
		transform.position = new Vector3(-.5f, .5f, -.5f);

		_voxelData.AddVoxel(new Vector3(0, 0, 0), Color.white);
	}

	private void Start() 
	{
		_voxelRenderer.RenderMesh(_voxelData);
	}

    /// <summary>
    /// Add a voxel to the model at a location normally adjacent to the given raycast hit data
    /// </summary>
	public void AddVoxel(RaycastHit hitData, Color color) 
	{
		_voxelData.AddVoxel(GetAdjacentBlockPos(hitData), color);
		_voxelRenderer.RenderMesh(_voxelData);
	}

    /// <summary>
    /// Gets the block normally adjacent to the raycast hit data
    /// </summary>
	private Vector3 GetAdjacentBlockPos(RaycastHit hit) 
	{
    	return new Vector3(MoveWithinBlock(hit.point.x, hit.normal.x, true),
            MoveWithinBlock(hit.point.y, hit.normal.y, true),
            MoveWithinBlock(hit.point.z, hit.normal.z, true));
	}

    /// <summary>
    /// Gets the block at the raycast hit data
    /// </summary>
    private Vector3 GetBlockPos(RaycastHit hit)
    {
    	return new Vector3(MoveWithinBlock(hit.point.x, hit.normal.x, false),
            MoveWithinBlock(hit.point.y, hit.normal.y, false),
            MoveWithinBlock(hit.point.z, hit.normal.z, false));
    }
 
    private float MoveWithinBlock(float pos, float norm, bool adjacent)
     {
        if (pos - (int)pos == 0.5f || pos - (int)pos == -0.5f)
        {
        	pos += (adjacent ? 1 : -1) * (norm / 2);
        }
        return pos;
     }
 }

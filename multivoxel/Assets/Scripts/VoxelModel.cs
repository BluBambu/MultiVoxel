using System.Collections;
using UnityEngine;

// Represents the voxel model in the editor
[RequireComponent(typeof(VoxelModelRenderer))]
public class VoxelModel : MonoBehaviour
{
    private const float VoxelTransitionTime = 5f;

	private VoxelModelRenderer _voxelRenderer;
	private VoxelData _voxelData;

	private void Awake() 
	{
		_voxelRenderer = GetComponent<VoxelModelRenderer>();
		_voxelData = new VoxelData();
		transform.position = new Vector3(-.5f, .5f, -.5f);
	}

	private void Start() 
	{
        _voxelRenderer.RenderMesh(_voxelData);
	}

    public void AddVoxel(Vector3Int pos, Color color)
    {
        StartCoroutine(AddVoxelCor(pos, color));
    } 

    public void RemoveVoxel(Vector3Int pos)
    {
        _voxelData.RemoveVoxel(pos);
        _voxelRenderer.RenderMesh(_voxelData);
    }

    // TODO: Change the shader
    private IEnumerator AddVoxelCor(Vector3Int pos, Color color)
    {
        Transform tempVoxel = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
        tempVoxel.position = (Vector3) pos;
        tempVoxel.localScale = Vector3.zero;
        tempVoxel.GetComponent<MeshRenderer>().material.color = color;
        while (tempVoxel.localScale.x < .95f)
        {
            tempVoxel.localScale = Vector3.Lerp(tempVoxel.localScale, Vector3.one, .1f);
            yield return null;
        }
        Destroy(tempVoxel.gameObject);
        _voxelData.AddVoxel(pos, color);
        _voxelRenderer.RenderMesh(_voxelData);
    }
 }

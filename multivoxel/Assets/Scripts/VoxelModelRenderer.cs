using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Handles rendering and managing the mesh collider of the voxel model
/// </summary>
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class VoxelModelRenderer : MonoBehaviour 
{
	private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;

	private void Awake() 
	{
        _meshCollider = GetComponent<MeshCollider>();
		_meshFilter = GetComponent<MeshFilter>();
	}

    // TODO: Clean all of this code up...

    /// <summary>
    /// Update the model to represent the given voxel data
    /// </summary>
	public void RenderMesh(VoxelData voxelData) 
	{
		List<Vector3> verts = new List<Vector3>();
		List<int> trigs = new List<int>();
		List<Color> colors = new List<Color>();
		Mesh mesh = _meshFilter.mesh;

        foreach (Voxel voxel in voxelData.Voxels)
        {
            if (!voxelData.HasVoxelAtPos(voxel.Pos + Vector3Int.Up))
            {
                GenTopFace(voxel, verts, trigs, colors);
            }
            if (!voxelData.HasVoxelAtPos(voxel.Pos - Vector3Int.Up))
            {
                GenBottomFace(voxel, verts, trigs, colors);
            }
            if (!voxelData.HasVoxelAtPos(voxel.Pos + Vector3Int.Forward))
            {
                GenNorthFace(voxel, verts, trigs, colors);
            }
            if (!voxelData.HasVoxelAtPos(voxel.Pos - Vector3Int.Forward))
            {
                GenSouthFace(voxel, verts, trigs, colors);
            }
            if (!voxelData.HasVoxelAtPos(voxel.Pos + Vector3Int.Right))
            { 
                GenEastFace(voxel, verts, trigs, colors);
            }
            if (!voxelData.HasVoxelAtPos(voxel.Pos - Vector3Int.Right))
            {
                GenWestFace(voxel, verts, trigs, colors);
            }
        }

        // Update the mesh filter to the new model
		mesh.Clear();
		mesh.vertices = verts.ToArray();
		mesh.triangles = trigs.ToArray();
		mesh.colors = colors.ToArray();
		mesh.uv = new List<Vector2>(verts.Count).ToArray();
		mesh.Optimize();
		mesh.RecalculateNormals();

        // Update the mesh collider to the new model
        _meshCollider.sharedMesh = null;
        _meshCollider.sharedMesh = mesh;
	}

    public MeshFilter MeshFilter { get { return _meshFilter; } }

	private void GenTopFace(Voxel voxel, List<Vector3> verts, List<int> trigs, List<Color> colors)
    {
        verts.Add(new Vector3(voxel.Pos.X, voxel.Pos.Y, voxel.Pos.Z + 1));
        verts.Add(new Vector3(voxel.Pos.X + 1, voxel.Pos.Y, voxel.Pos.Z + 1));
        verts.Add(new Vector3(voxel.Pos.X + 1, voxel.Pos.Y, voxel.Pos.Z));
        verts.Add(new Vector3(voxel.Pos.X, voxel.Pos.Y, voxel.Pos.Z));
        InitFace(verts, trigs, colors, voxel.Color);
    }

    private void GenBottomFace(Voxel voxel, List<Vector3> verts, List<int> trigs, List<Color> colors)
    {
        verts.Add(new Vector3(voxel.Pos.X, voxel.Pos.Y - 1, voxel.Pos.Z));
        verts.Add(new Vector3(voxel.Pos.X + 1, voxel.Pos.Y - 1, voxel.Pos.Z));
        verts.Add(new Vector3(voxel.Pos.X + 1, voxel.Pos.Y - 1, voxel.Pos.Z + 1));
        verts.Add(new Vector3(voxel.Pos.X, voxel.Pos.Y - 1, voxel.Pos.Z + 1));
        InitFace(verts, trigs, colors, voxel.Color);
    }

    private void GenNorthFace(Voxel voxel, List<Vector3> verts, List<int> trigs, List<Color> colors)
    {
        verts.Add(new Vector3(voxel.Pos.X + 1, voxel.Pos.Y - 1, voxel.Pos.Z + 1));
        verts.Add(new Vector3(voxel.Pos.X + 1, voxel.Pos.Y, voxel.Pos.Z + 1));
        verts.Add(new Vector3(voxel.Pos.X, voxel.Pos.Y, voxel.Pos.Z + 1));
        verts.Add(new Vector3(voxel.Pos.X, voxel.Pos.Y - 1, voxel.Pos.Z + 1));
        InitFace(verts, trigs, colors, voxel.Color);
    }

    private void GenEastFace(Voxel voxel, List<Vector3> verts, List<int> trigs, List<Color> colors)
    {
        verts.Add(new Vector3(voxel.Pos.X + 1, voxel.Pos.Y - 1, voxel.Pos.Z));
        verts.Add(new Vector3(voxel.Pos.X + 1, voxel.Pos.Y, voxel.Pos.Z));
        verts.Add(new Vector3(voxel.Pos.X + 1, voxel.Pos.Y, voxel.Pos.Z + 1));
        verts.Add(new Vector3(voxel.Pos.X + 1, voxel.Pos.Y - 1, voxel.Pos.Z + 1));
        InitFace(verts, trigs, colors, voxel.Color);
    }

    private void GenSouthFace(Voxel voxel, List<Vector3> verts, List<int> trigs, List<Color> colors)
    {
        verts.Add(new Vector3(voxel.Pos.X, voxel.Pos.Y - 1, voxel.Pos.Z));
        verts.Add(new Vector3(voxel.Pos.X, voxel.Pos.Y, voxel.Pos.Z));
        verts.Add(new Vector3(voxel.Pos.X + 1, voxel.Pos.Y, voxel.Pos.Z));
        verts.Add(new Vector3(voxel.Pos.X + 1, voxel.Pos.Y - 1, voxel.Pos.Z));
        InitFace(verts, trigs, colors, voxel.Color);
    }

    private void GenWestFace(Voxel voxel, List<Vector3> verts, List<int> trigs, List<Color> colors)
    {
        verts.Add(new Vector3(voxel.Pos.X, voxel.Pos.Y - 1, voxel.Pos.Z + 1));
        verts.Add(new Vector3(voxel.Pos.X, voxel.Pos.Y, voxel.Pos.Z + 1));
        verts.Add(new Vector3(voxel.Pos.X, voxel.Pos.Y, voxel.Pos.Z));
        verts.Add(new Vector3(voxel.Pos.X, voxel.Pos.Y - 1, voxel.Pos.Z));
        InitFace(verts, trigs, colors, voxel.Color);
    }

    private void InitFace(List<Vector3> verts, List<int> trigs, List<Color> colors, Color color) 
    {
        trigs.Add(verts.Count - 4);
        trigs.Add(verts.Count - 3);
        trigs.Add(verts.Count - 2);

        trigs.Add(verts.Count - 4);
        trigs.Add(verts.Count - 2);
        trigs.Add(verts.Count - 1);

        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
    }
}

using UnityEngine;
using System.Collections.Generic;

// Holds voxel data
public class VoxelData 
{
	public IEnumerable<Voxel> Voxels 
	{
		get { return _data.Values; }
	}

	private readonly Dictionary<Vector3Int, Voxel> _data;

    public VoxelData() 
    {
    	_data = new Dictionary<Vector3Int, Voxel>();
        AddVoxel(Vector3Int.Zero, Color.white);
    }

    public VoxelData(Voxel[] saveData)
    {
    	_data = new Dictionary<Vector3Int, Voxel>();
        foreach (Voxel voxel in saveData)
        {
            _data[voxel.Pos] = voxel;
        }
    }

    // Will override any previous voxel that exists at the given pos
    public void AddVoxel(Vector3Int pos, Color color) 
    {
    	_data[pos] = new Voxel(pos, color);
    }

    // Does nothing if there is no voxel at the given pos or if we're removing the origin block
    public void RemoveVoxel(Vector3Int pos)
    {
        if (_data.ContainsKey(pos) && pos != Vector3Int.Zero)
        {
            _data.Remove(pos);
        }
    }

    public bool HasVoxelAtPos(Vector3Int pos) 
    {
    	return _data.ContainsKey(pos);
    }
}

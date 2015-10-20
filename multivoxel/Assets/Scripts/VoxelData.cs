using UnityEngine;
using System.Collections.Generic;

public struct VoxelSaveData
{
    internal Voxel[] _data;

    internal VoxelSaveData(Voxel[] data)
    {
        _data = data;
    }
}

/// <summary>
/// Holds voxel data
/// </summary>
public class VoxelData 
{
	public IEnumerable<Voxel> Voxels 
	{
		get { return _data.Values; }
	}

	private Dictionary<Vector3, Voxel> _data;

    /// <summary>
    /// Construct a new set of voxel data
    /// </summary>
    public VoxelData() 
    {
    	_data = new Dictionary<Vector3, Voxel>();
    }

    public VoxelData(VoxelSaveData saveData) : base()
    {
        foreach (Voxel voxel in saveData._data)
        {
            _data[voxel.Pos] = voxel;
        }
    }

    public void AddVoxel(Vector3 pos, Color color) 
    {
    	_data[Utils.RoundVector3(pos)] = new Voxel(Utils.RoundVector3(pos), color);
    }

    public bool HasVoxelAtPos(Vector3 pos) 
    {
    	return _data.ContainsKey(Utils.RoundVector3(pos));
    }

    public VoxelSaveData GetSaveData()
    {
        Voxel[] saveData = new Voxel[_data.Count];
        _data.Values.CopyTo(saveData, 0);
        return new VoxelSaveData(saveData);
    }
}

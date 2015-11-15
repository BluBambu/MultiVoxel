using UnityEngine;
using System.Collections.Generic;

// Holds voxel data
public class VoxelData 
{
	public IEnumerable<Voxel> Voxels 
	{
		get { return _data.Values; }
	}

	private readonly Dictionary<Vector3, Voxel> _data;

    public VoxelData() 
    {
    	_data = new Dictionary<Vector3, Voxel>();
    }

    public VoxelData(Voxel[] saveData) : base()
    {
        foreach (Voxel voxel in saveData)
        {
            _data[voxel.Pos] = voxel;
        }
    }

    // Will override any previous voxel that exists at the given pos
    public void AddVoxel(Vector3 pos, Color color) 
    {
    	_data[Utils.RoundVector3(pos)] = new Voxel(Utils.RoundVector3(pos), color);
    }

    // Does nothing if there is no voxel at the given pos or if we're removing the origin block
    public void RemoveVoxel(Vector3 pos)
    {
        pos = Utils.RoundVector3(pos);
        if (_data.ContainsKey(pos) && pos != Vector3.zero)
        {
            _data.Remove(pos);
        }
    }

    public bool HasVoxelAtPos(Vector3 pos) 
    {
    	return _data.ContainsKey(Utils.RoundVector3(pos));
    }
}

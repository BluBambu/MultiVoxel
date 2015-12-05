using UnityEngine;

[System.Serializable]
public class VoxelCommand {
	private enum Type {
		Add,
		Remove,
	}
	private Type _type;
	private Voxel _voxel;
	private Vector3Int _pos;

	public static VoxelCommand Add(Voxel voxel) {
		VoxelCommand cmd = new VoxelCommand ();
		cmd._type = Type.Add;
		cmd._voxel = voxel;
		return cmd;
	}

	public static VoxelCommand Remove(Vector3Int pos) {
		VoxelCommand cmd = new VoxelCommand ();
		cmd._type = Type.Remove;
		cmd._pos = pos;
		return cmd;
	}

	public void Apply (VoxelData voxelData) {
		switch (_type) {
		case Type.Add:
			voxelData.AddVoxel(_voxel);
			break;
		case Type.Remove:
			voxelData.RemoveVoxel(_pos);
			break;
		default:
			throw new UnityException();
		}
	}
}

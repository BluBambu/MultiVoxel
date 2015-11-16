using UnityEngine;

/// <summary>
/// Represents a single immutable voxel
/// </summary>
public struct Voxel {
	public readonly Color Color;
	public readonly Vector3Int Pos;

	public Voxel(Vector3Int pos, Color color) {
		Color = color;
		Pos = pos;
	}
}

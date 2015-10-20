using UnityEngine;

/// <summary>
/// Represents a single immutable voxel
/// </summary>
public struct Voxel {
	public readonly Color Color;
	public readonly Vector3 Pos;

	public Voxel(Vector3 pos, Color color) {
		Color = color;
		Pos = pos;
	}
}

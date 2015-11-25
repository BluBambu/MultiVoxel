using System;
using UnityEngine;

/// <summary>
/// Represents a single immutable voxel
/// </summary>
[Serializable]
public struct Voxel {
	public readonly Vector3Int Pos;

    // Color hack required because UnityEngine.Color isn't seralizeable
    private readonly float _r;
    private readonly float _g;
    private readonly float _b;
    private readonly float _a;

    // TODO: Cache this...
    public Color Color
    {
        get { return new Color(_r, _g, _b, _a); }
    }

	public Voxel(Vector3Int pos, Color color)
	{
		Pos = pos;
        _r = color.r;
	    _g = color.g;
	    _b = color.b;
	    _a = color.a;
	}
}

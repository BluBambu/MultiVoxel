using System;
using UnityEngine;

[Serializable]
public struct Vector3Int
{
    public static readonly Vector3Int Zero = new Vector3Int();
    public static readonly Vector3Int One = new Vector3Int(1, 1, 1);
    public static readonly Vector3Int Up = new Vector3Int(0, 1, 0);
    public static readonly Vector3Int Forward = new Vector3Int(0, 0, 1);
    public static readonly Vector3Int Right = new Vector3Int(1, 0, 0);

    public readonly int X;
    public readonly int Y;
    public readonly int Z;

    public Vector3Int(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public override bool Equals(object obj)
    {
        if (obj is Vector3Int)
        {
            Vector3Int v = (Vector3Int) obj;
            return this == v;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return X + Y * 31 + Z * 157;
    }

    public override string ToString()
    {
        return string.Format("({0}, {1}, {2})", X, Y, Z);
    }

    // Vector3 -> Vector3Int
    // Round the Vector3 components to the nearest integer
    public static explicit operator Vector3Int(Vector3 v)
    {
        return new Vector3Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), 
            Mathf.RoundToInt(v.z));
    }

    // Vector3Int -> Vector3
    public static explicit operator Vector3(Vector3Int v)
    {
        return new Vector3(v.X, v.Y, v.Z);
    }

    public static bool operator ==(Vector3Int v1, Vector3Int v2)
    {
        return v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z;
    }

    public static bool operator !=(Vector3Int v1, Vector3Int v2)
    {
        return !(v1 == v2);
    }

    public static Vector3Int operator -(Vector3Int v)
    {
        return new Vector3Int(-v.X, -v.Y, -v.Z);
    }
    public static Vector3Int operator +(Vector3Int v1, Vector3Int v2)
    {
        return new Vector3Int(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
    }

    public static Vector3Int operator -(Vector3Int v1, Vector3Int v2)
    {
        return v1 + -v2;
    }
}
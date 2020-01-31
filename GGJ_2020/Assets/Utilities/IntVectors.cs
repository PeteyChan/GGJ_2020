using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct int2 : System.IEquatable<int2>
{
    public int x;
    public int y;

    public int2(int x, int y)
    {
        this.x = x; this.y = y;
    }

    public override bool Equals(object obj)
    {
        if (obj is int2 value)
            return x == value.x && y == value.y;
        return false;
    }

    public override string ToString()
        => $"int2({x}, {y})";

    public override int GetHashCode()
        => x * 53 + y * 3079;

    public static implicit operator Vector3(int2 obj)
        => new Vector3(obj.x, 0, obj.y);

    public bool Equals(int2 other)
        => this == other;

    public static bool operator ==(int2 a, int2 b)
        => a.x == b.x && a.y == b.y;

    public static bool operator !=(int2 a, int2 b)
        => !(a == b);

    public static int2 operator +(int2 a, int2 b)
        => new int2(a.x + b.x, a.y + b.y);

    public static int2 operator -(int2 a, int2 b)
        => new int2(a.x - b.x, a.y - b.y);

    public static implicit operator Vector2(int2 int2)
        => new Vector2(int2.x, int2.y);

    public static implicit operator int2(Vector2 vector2)
        => new int2(Mathf.RoundToInt(vector2.x), Mathf.RoundToInt(vector2.y));
}

[System.Serializable]
public struct int3 : System.IEquatable<int3>
{
    public int x;
    public int y;
    public int z;

    public int3(int x, int y, int z)
    {
        this.x = x; this.y = y; this.z = z;
    }

    public override bool Equals(object obj)
    {
        if (obj is int2 value)
            return x == value.x && y == value.y;
        return false;
    }

    public override string ToString()
        => $"int3({x}, {y}, {z})";

    public override int GetHashCode()
        => x * 53 + y * 3079 + z * 196613;

    public bool Equals(int3 other)
        => this == other;

    public static bool operator ==(int3 a, int3 b)
        => a.x == b.x && a.y == b.y && a.z == b.z;

    public static bool operator !=(int3 a, int3 b)
        => !(a == b);

    public static int3 operator +(int3 a, int3 b)
        => new int3(a.x + b.x, a.y + b.y, a.z + b.z);

    public static int3 operator -(int3 a, int3 b)
        => new int3(a.x - b.x, a.y - b.y, a.z - b.z);

    public static implicit operator Vector3(int3 value)
        => new Vector3(value.x, value.y, value.z);

    public static implicit operator int3(Vector3 value)
        => new int3(
            Mathf.RoundToInt(value.x),
            Mathf.RoundToInt(value.y),
            Mathf.RoundToInt(value.z));
}
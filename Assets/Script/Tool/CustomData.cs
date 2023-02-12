using UnityEngine;
using System;

[Serializable]
public enum NodeType
{
    TypeRoot,
    Type1,
    Type2,
}

[Serializable]
public enum ConnectType
{
    None,
    In,
    Out,
}

public enum ItemType
{
    Node,
}

[Serializable]
public struct Vector2Serializable
{
    private float x;
    private float y;

    public Vector2Serializable(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public Vector2 Value
    {
        get { return new Vector2(x, y); }

        set
        {
            x = value.x;
            y = value.y;
        }
    }
}

[Serializable]
public struct RectSerializable
{
    private float x;
    private float y;
    private float width;
    private float height;

    public RectSerializable(float x, float y, float width, float height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }

    public Rect Value
    {
        get { return new Rect(x, y, width, height); }

        set
        {
            x = value.x;
            y = value.y;
            width = value.width;
            height = value.height;
        }
    }
}
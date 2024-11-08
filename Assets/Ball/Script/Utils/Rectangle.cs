using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rectangle
{
    public float X { get; private set; }
    public float Y { get; private set; }
    public float Width { get; private set; }
    public float Height { get; private set; }

    public float Top { get; private set; }
    public float Bottom { get; private set; }
    public float Left { get; private set; }
    public float Right { get; private set; }

    public Rectangle(float x, float y, float width, float height)
    {
        this.X = x;
        this.Y = y;
        this.Width = width;
        this.Height = height;

        UpdateEdges();
    }

    private void UpdateEdges()
    {
        Top = Y + Height / 2;
        Bottom = Y - Height / 2;
        Left = X - Width / 2;
        Right = X + Width / 2;

    }
}
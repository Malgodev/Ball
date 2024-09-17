using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle
{
    public float radius { get; private set; }
    public Vector2 centerPosition { get; private set; }

    public Circle(float radius, Vector2 centerPosition)
    {
        this.radius = radius;
        this.centerPosition = centerPosition;
    }
}

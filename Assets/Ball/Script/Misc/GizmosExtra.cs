using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmosExtra
{
    private const float GIZMO_DISK_THICKNESS = 0.01f;

    public static void DrawWireDisk(Vector3 position, float radius, Color color)
    {
        Color oldColor = Gizmos.color;
        Gizmos.color = color;
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(position, Quaternion.identity, new Vector3(1, GIZMO_DISK_THICKNESS, 1));
        Gizmos.DrawWireSphere(Vector3.zero, radius);
        Gizmos.matrix = oldMatrix;
        Gizmos.color = oldColor;
    }

    public static void DrawWireRectangle(Vector3 position, float  width, float height, Color color)
    {
        Color oldColor = Gizmos.color;

        Gizmos.color = color;

        Matrix4x4 oldMatrix = Gizmos.matrix;

        Gizmos.matrix = Matrix4x4.TRS(position, Quaternion.identity, new Vector3(width, height, 1));

        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);

        Gizmos.matrix = oldMatrix;

        Gizmos.color = oldColor;
    }
}

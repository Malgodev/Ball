using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Vector2 AsVector2(Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }

    // public static Vector2 LerpVector()

    public static Vector2 ClosestPositionToRect(Vector2 gamePosition, Vector2 gameScale, GameObject rect)
    {
        Vector2 rectPosition = rect.transform.position;
        Vector2 rectScale = rect.transform.localScale;

        Vector2 rectMin = rectPosition - rectScale / 2;
        Vector2 rectMax = rectPosition + rectScale / 2;

        float clampedX = Mathf.Clamp(gamePosition.x, rectMin.x, rectMax.x);
        float clampedY = Mathf.Clamp(gamePosition.y, rectMin.y, rectMax.y);

        return new Vector2(clampedX, clampedY);
    }

    public static bool IsGameObjectInsideRect(Vector2 gamePosition, Vector2 gameScale, GameObject rect)
    {
        return IsGameObjectInsideRect(gamePosition, gameScale,
            rect.transform.position, rect.transform.localScale);
    }

    public static bool IsGameObjectInsideRect(GameObject gameObject, GameObject rect)
    {
        return IsGameObjectInsideRect(gameObject.transform.position, gameObject.transform.localScale,
            rect.transform.position, rect.transform.localScale);
    }

    public static bool IsGameObjectInsideRect(GameObject gameObject, Vector2 rectPosition, Vector2 rectScale)
    {
        return IsGameObjectInsideRect(gameObject.transform.position, gameObject.transform.localScale, rectPosition, rectScale);
    }

    public static bool IsGameObjectInsideRect(Vector2 gamePosition, Vector2 gameScale, Vector2 rectPosition, Vector2 rectScale)
    {
        Vector2 gameMin = gamePosition - gameScale / 2;
        Vector2 gameMax = gamePosition + gameScale / 2;

        Vector2 rectMin = rectPosition - rectScale / 2;
        Vector2 rectMax = rectPosition + rectScale / 2;

        return gameMin.x >= rectMin.x && gameMax.x <= rectMax.x &&
               gameMin.y >= rectMin.y && gameMax.y <= rectMax.y;
    }

    public static bool IsInsideRectangle(Vector2 point, Vector2 rectPosition, Vector2 rectScale)
    {
        Vector2 rectMin = rectPosition - rectScale / 2;
        Vector2 rectMax = rectPosition + rectScale / 2;

        return point.x >= rectMin.x && point.x <= rectMax.x &&
               point.y >= rectMin.y && point.y <= rectMax.y;
    }
}

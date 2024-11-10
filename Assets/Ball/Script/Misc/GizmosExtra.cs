# if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Miscellaneous
{
    public static class GizmosExtra
    {
        private const float GIZMO_DISK_THICKNESS = 0.01f;

        public static void DrawWireDisk(Vector3 position, float radius, Color color)
        {
            Color oldColor = Gizmos.color;
            Gizmos.color = color;
            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(position, Quaternion.identity, new Vector3(1, 1, GIZMO_DISK_THICKNESS));
            Gizmos.DrawWireSphere(Vector3.zero, radius);
            Gizmos.matrix = oldMatrix;
            Gizmos.color = oldColor;
        }

        public static void DrawWireRectangle(Vector3 position, float width, float height, Color color)
        {
            Color oldColor = Gizmos.color;

            Gizmos.color = color;

            Matrix4x4 oldMatrix = Gizmos.matrix;

            Gizmos.matrix = Matrix4x4.TRS(position, Quaternion.identity, new Vector3(width, height, 1));

            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);

            Gizmos.matrix = oldMatrix;

            Gizmos.color = oldColor;
        }

        public static void DrawString(string text, Vector3 worldPos, Color? textColor = null, Color? backgroundColor = null)
        {
            UnityEditor.Handles.BeginGUI();
            var restoreTextColor = GUI.color;
            var restoreBackColor = GUI.backgroundColor;

            GUI.color = textColor ?? Color.white;
            GUI.backgroundColor = backgroundColor ?? Color.black;

            var view = UnityEditor.SceneView.currentDrawingSceneView;
            if (view != null && view.camera != null)
            {
                Vector3 screenPos = view.camera.WorldToScreenPoint(worldPos);
                if (screenPos.y < 0 || screenPos.y > Screen.height || screenPos.x < 0 || screenPos.x > Screen.width || screenPos.z < 0)
                {
                    GUI.color = restoreTextColor;
                    UnityEditor.Handles.EndGUI();
                    return;
                }
                Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text));
                var r = new Rect(screenPos.x - (size.x / 2), -screenPos.y + view.position.height + 4, size.x, size.y);
                GUI.Box(r, text, EditorStyles.numberField);
                GUI.Label(r, text);
                GUI.color = restoreTextColor;
                GUI.backgroundColor = restoreBackColor;
            }
            UnityEditor.Handles.EndGUI();
        }
    }

}
#endif
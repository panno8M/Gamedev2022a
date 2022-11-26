using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class GizmosEx
{
#if UNITY_EDITOR
  static System.Type lastwindow;
  public static void DrawArrow(Vector3 from, Vector3 to)
  {
    Gizmos.DrawLine(from, to);

    if (SceneView.focusedWindow?.GetType() == typeof(SceneView)
     || SceneView.focusedWindow?.GetType() == typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView"))
    {
      lastwindow = SceneView.focusedWindow?.GetType();
    }
    var forward = (
        lastwindow == typeof(SceneView)
            ? SceneView.GetAllSceneCameras()[0]
            : Camera.main)
        .transform.forward;

    var arrowEdge = (from - to).normalized / 2;
    var angle1 = Quaternion.AngleAxis(15, forward);
    var angle2 = Quaternion.AngleAxis(-15, forward);
    var arrow1 = to + angle1 * arrowEdge;
    var arrow2 = to + angle2 * arrowEdge;
    Gizmos.DrawLine(to, arrow1);
    Gizmos.DrawLine(to, arrow2);
    Gizmos.DrawLine(arrow1, arrow2);

  }
#endif
}

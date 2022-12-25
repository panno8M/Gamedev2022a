using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Utilities
{
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
    private static int _circleVertexCount = 64;
    public static void DrawWireCircle(Vector3 center, Quaternion rotation, float radius)
    {
      DrawWireRegularPolygon(_circleVertexCount, center, rotation, radius);
    }

    public static void DrawWireRegularPolygon(int vertexCount, Vector3 center, Quaternion rotation, float radius)
    {
      if (vertexCount < 3) { return; }

      Vector3 previousPosition = Vector3.zero;

      float step = 2f * Mathf.PI / vertexCount;
      float offset = Mathf.PI * 0.5f + ((vertexCount % 2 == 0) ? step * 0.5f : 0f);

      for (int i = 0; i <= vertexCount; i++)
      {
        float theta = step * i + offset;

        float x = radius * Mathf.Cos(theta);
        float y = radius * Mathf.Sin(theta);

        Vector3 nextPosition = center + rotation * new Vector3(x, y, 0f);

        if (i == 0)
        { previousPosition = nextPosition; }
        else
        { Gizmos.DrawLine(previousPosition, nextPosition); }

        previousPosition = nextPosition;
      }
    }
#endif
  }
}
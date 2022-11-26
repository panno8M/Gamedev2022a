using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assembly.Components.StageGimmicks
{
  public class Portal : MonoBehaviour
  {

    [SerializeField] Portal _next;
    [SerializeField] PortalKind _kind;

    public Portal next => _next;
    public PortalKind kind => _kind;

    public Vector3 positionDelta => _next
      ? _next.transform.position - transform.position
      : Vector3.zero;

    public bool Handshake(ITransferable item)
    {
      return enabled && next;
    }

    public void ReadyTransfer(ITransferable item)
    {
      item.Transfer(this);
      item.OnCompleteTransfer(this);
    }

    void OnTriggerEnter(Collider other)
    {
      other.GetComponent<ITransferable>()?.OnPortalEnter(this);
    }
    void OnTriggerExit(Collider other)
    {
      other.GetComponent<ITransferable>()?.OnPortalExit(this);

    }

#if UNITY_EDITOR

    Color color;
    void OnDrawGizmos()
    {
      if (color == default(Color))
      {
        color = Random.ColorHSV(0, 1, 1, 1, 1, 1);
      }
      Gizmos.color = color;
      if (_next)
      {
        Gizmos.DrawIcon(transform.position, "Check", true);
        GizmosEx.DrawArrow(transform.position, _next.transform.position);
      }
      else
      {
        Gizmos.DrawIcon(transform.position, "Cross", true);
      }
    }

    [CustomEditor(typeof(Portal))]
    public class PortalEditor : Editor
    {
      public override void OnInspectorGUI()
      {
        Portal portal = target as Portal;

        Portal newNext = EditorGUILayout.ObjectField("Next Portal", portal._next, typeof(Portal), true) as Portal;
        PortalKind newKind = (PortalKind)(EditorGUILayout.EnumPopup("Kind", portal._kind));
        if (newNext != portal._next || newKind != portal._kind)
        {
          portal._next = newNext;
          portal._kind = newKind;
          if (portal._next && portal._next.kind != portal.kind)
          {
            Debug.LogWarningFormat("({0}) {1}, the next portal, have not same kind of", portal.name, portal._next.name);
          }
        }
      }
    }
#endif
  }
}
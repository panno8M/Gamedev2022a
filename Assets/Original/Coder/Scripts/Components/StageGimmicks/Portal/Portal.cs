using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Utilities;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assembly.Components.StageGimmicks
{
  public class Portal : MonoBehaviour
  {

    [SerializeField] protected Portal _next;
    [SerializeField] protected PortalKind _kind;

    public Portal next => _next;
    public PortalKind kind => _kind;

    public Vector3 positionDelta => _next
      ? _next.transform.position - transform.position
      : Vector3.zero;

    public bool Handshake(ITransferable item)
    {
      return enabled && next;
    }

    public async UniTask Transfer(ITransferable item, CancellationToken token = default(CancellationToken))
    {
      if (item == null || !Handshake(item) || !item.Handshake(this)) { return; }
      if (item != null) await item.StartTransfer(this, token);
      if (item != null) await item.ProcessTransfer(this, token);
      if (item != null) await item.CompleteTransfer(this, token);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
      var x = other.GetComponent<ITransferable>();
      if (x != null) x.closestPortal = this;
    }
    protected virtual void OnTriggerExit(Collider other)
    {
      var x = other.GetComponent<ITransferable>();
      if (x != null && x.closestPortal == this) x.closestPortal = null;
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

    [CustomEditor(typeof(Portal), true)]
    public class PortalEditor : Editor
    {
      public override void OnInspectorGUI()
      {
        Portal portal = target as Portal;
        var oldNext = portal._next;
        var oldKind = portal._kind;

        base.OnInspectorGUI();

        if (oldNext != portal._next || oldKind != portal._kind)
        {
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
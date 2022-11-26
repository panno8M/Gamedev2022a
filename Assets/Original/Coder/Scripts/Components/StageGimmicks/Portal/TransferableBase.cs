using UnityEngine;
using UniRx;

namespace Assembly.Components.StageGimmicks
{
  [RequireComponent(typeof(Rigidbody))]
  public abstract class TransferableBase : MonoBehaviour, ITransferable
  {
    [SerializeField] protected Rigidbody rb;
    protected Portal nearestPortal;
    bool _isTransfering;

    protected virtual bool Handshake(Portal portal) { return true; }

    bool Connectable(Portal portal)
    {
      return !_isTransfering && portal &&
          Handshake(portal) && portal.Handshake(this);
    }

    protected abstract void OnStartTransfer(Portal portal);
    protected abstract void OnCompleteTransfer(Portal portal);

    protected void DoneStart() { nearestPortal.ReadyTransfer(this); }
    protected void DoneComplete() { _isTransfering = false; }

    public void StartTransfer(Portal portal)
    {
      if (_isTransfering) { return; }
      _isTransfering = true;
      OnStartTransfer(portal);
    }
    public virtual void ProcessTransfer(Portal portal)
    {
      ProcessTransfer_SamePoint(portal);
    }
    public void CompleteTransfer(Portal portal)
    {
      if (!_isTransfering) { return; }
      OnCompleteTransfer(portal);
    }

    protected void ProcessTransfer_SamePoint(Portal portal)
    {
      rb.MovePosition(transform.position + portal.positionDelta);
    }
    protected void ProcessTransfer_Center(Portal portal)
    {
      rb.MovePosition(portal.next.transform.position);
    }

    protected void Transition(Unit _) { Transition(); }
    protected void Transition() { Transition(nearestPortal); }
    protected void Transition(Portal portal)
    {
      if (!Connectable(portal)) { return; }
      StartTransfer(portal);
    }

    public virtual void OnPortalEnter(Portal portal)
    {
      if (_isTransfering) { return; }
      nearestPortal = portal;
    }
    public virtual void OnPortalExit(Portal portal)
    {
      if (_isTransfering) { return; }
      nearestPortal = null;
    }
  }
}
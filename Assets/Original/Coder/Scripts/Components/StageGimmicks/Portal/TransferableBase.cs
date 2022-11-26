using UnityEngine;
using UniRx;

public abstract class TransferableBase : MonoBehaviour, ITransferable
{
  [SerializeField] protected Portal nearestPortal;

  protected virtual bool Handshake(Portal portal)
  {
    return portal.Handshake(this);
  }

  protected abstract void OnStartTransfer(Portal portal);
  public abstract void OnCompleteTransfer(Portal portal);

  public virtual void Transfer(Portal portal)
  {
    transform.position += portal.positionDelta;
  }

  protected void Transision(Unit _) { Transision(); }
  protected void Transision() { Transision(nearestPortal); }
  protected virtual void Transision(Portal portal)
  {
    if (!Handshake(portal)) { return; }
    OnStartTransfer(portal);
  }

  public virtual void OnPortalEnter(Portal portal)
  {
    if (portal == nearestPortal?.next)
    {
      nearestPortal = null;
      return;
    }
    nearestPortal = portal;
  }
  public virtual void OnPortalExit(Portal portal)
  {
  }
}

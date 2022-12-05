using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;

namespace Assembly.Components.StageGimmicks
{
  public abstract class TransferableBase : MonoBehaviour, ITransferable
  {
    ReactiveProperty<Portal> _ClosestPortal = new ReactiveProperty<Portal>();
    public Portal closestPortal
    {
      set
      {
        _ClosestPortal.Value = value;
      }
      get
      {
        return _ClosestPortal.Value;
      }
    }
    public IObservable<Portal> OnPortalOverrap => _ClosestPortal.Where(x => x);

    Portal _processingPortal;
    protected Portal processingPortal => _processingPortal;


    protected virtual UniTask OnStartTransfer(Portal portal) { return UniTask.CompletedTask; }
    protected abstract UniTask OnProcessTransfer(Portal portal);
    protected virtual UniTask OnCompleteTransfer(Portal portal) { return UniTask.CompletedTask; }
    protected virtual void AfterCompleteTransfer(Portal portal) { }

    public async UniTask StartTransfer(Portal portal)
    {
      if (processingPortal || !portal) { return; }
      _processingPortal = portal;
      await OnStartTransfer(portal);
    }
    public virtual async UniTask ProcessTransfer(Portal portal)
    {
      if (processingPortal != portal || !portal) { return; }
      await OnProcessTransfer(portal);
    }
    public async UniTask CompleteTransfer(Portal portal)
    {
      if (processingPortal != portal || !portal) { return; }
      await OnCompleteTransfer(portal);
      _processingPortal = null;
      AfterCompleteTransfer(portal);
    }

    protected void Transfer(Unit _) { closestPortal.Transfer(this).Forget(); }
    protected async UniTask Transfer() { await closestPortal.Transfer(this); }

    public bool Handshake(Portal portal)
    {
      return !processingPortal && portal && CustomCheck(portal);
    }
    protected virtual bool CustomCheck(Portal portal) { return true; }
  }
}
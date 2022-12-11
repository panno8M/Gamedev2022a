using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;

namespace Assembly.Components.StageGimmicks
{
  public abstract class TransferableBase : MonoBehaviour, ITransferable
  {
    ReactiveProperty<Portal> _ClosestPortal = new ReactiveProperty<Portal>();

    protected CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

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


    protected virtual UniTask OnStartTransfer(Portal portal, CancellationToken token) { return UniTask.CompletedTask; }
    protected abstract UniTask OnProcessTransfer(Portal portal, CancellationToken token);
    protected virtual UniTask OnCompleteTransfer(Portal portal, CancellationToken token) { return UniTask.CompletedTask; }
    protected virtual void AfterCompleteTransfer(Portal portal) { }

    public async UniTask StartTransfer(Portal portal, CancellationToken token)
    {
      if (processingPortal || !portal) { return; }
      _processingPortal = portal;
      await OnStartTransfer(portal, token);
    }
    public virtual async UniTask ProcessTransfer(Portal portal, CancellationToken token)
    {
      if (processingPortal != portal || !portal) { return; }
      await OnProcessTransfer(portal, token);
    }
    public async UniTask CompleteTransfer(Portal portal, CancellationToken token)
    {
      if (processingPortal != portal || !portal) { return; }
      await OnCompleteTransfer(portal, token);
      _processingPortal = null;
      AfterCompleteTransfer(portal);
    }

    protected void Transfer(Unit _)
    {
      closestPortal.Transfer(this, cancellationTokenSource.Token).Forget();
    }
    protected async UniTask Transfer()
    {
      await closestPortal.Transfer(this, cancellationTokenSource.Token);
    }

    public bool Handshake(Portal portal)
    {
      return !processingPortal && portal && CustomCheck(portal);
    }
    protected virtual bool CustomCheck(Portal portal) { return true; }
  }
}
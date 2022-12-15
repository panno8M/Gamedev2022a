using System.Threading;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using Assembly.Components.StageGimmicks;

namespace Assembly.Components.Actors.player
{
  public sealed class PlayerTransferable : TransferableBase
  {
#if DEBUG
    [SerializeField] bool mumbling;
#endif
    void Say(string msg)
    {
#if DEBUG
      if (mumbling)
        Debug.Log("<color=#D36D27FF>" + msg + "</color>");
#endif
    }

    [SerializeField] int _secTransferDurationEnter = 250;
    [SerializeField] int _secTransferDurationExit = 250;

    protected override void Blueprint()
    {
      throw new System.NotImplementedException();
    }

    void Start()
    {
      Global.Control.Interact
          .Where(_ => closestPortal && closestPortal.kind == PortalKind.Door)
          .Subscribe(Transfer);

      OnPortalOverrap.Subscribe(portal =>
      {
        Say("I found out the " + portal.kind + "!");
        if (portal.kind == PortalKind.Passage || portal.kind == PortalKind.Wormhole)
        {
          Transfer().Forget();
        }

      }).AddTo(this);
    }

    protected override async UniTask OnStartTransfer(Portal portal, CancellationToken token)
    {
      Say("Gotcha! This portal seems work!");
      await UniTask.Delay(_secTransferDurationEnter);
      Say("Let's go beyond!");
    }

    protected override UniTask OnProcessTransfer(Portal portal, CancellationToken token)
    {
      switch (portal.kind)
      {
        case PortalKind.Wormhole:
          ProcessTransfer_Center(portal);
          break;
        default:
          ProcessTransfer_SamePoint(portal);
          break;
      }
      return UniTask.CompletedTask;
    }

    protected override async UniTask OnCompleteTransfer(Portal portal, CancellationToken token)
    {
      Say("Wow, where is here...?");
      await UniTask.Delay(_secTransferDurationExit);
      Say("Oh, I know, I know here.");
    }

    void ProcessTransfer_SamePoint(Portal portal)
    {
      rigidbody.MovePosition(transform.position + portal.positionDelta);
    }
    void ProcessTransfer_Center(Portal portal)
    {
      rigidbody.MovePosition(portal.next.transform.position);
    }

  }
}
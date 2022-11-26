using System;
using System.Linq;
using UnityEngine;
using UniRx;
using Assembly.Components.StageGimmicks;

namespace Assembly.Components.Actors.player
{
  public sealed class PlayerTransferable : TransferableBase
  {
    void Say(string msg)
    {
#if DEBUG
      Debug.Log("<color=#D36D27FF>" + msg + "</color>");
#endif
    }

    [SerializeField] float _transferDurationEnter = 0.25f;
    [SerializeField] float _transferDurationExit = 0.25f;

    void Start()
    {
      Global.Control.Interact
          .Where(_ => nearestPortal && nearestPortal.kind == PortalKind.Door)
          .Subscribe(Transision);
    }

    public override void OnPortalEnter(Portal portal)
    {
      base.OnPortalEnter(portal);
      Say("I found out the " + portal.kind + "!");
      if (new[] { PortalKind.Passage, PortalKind.Wormhole }.Any(x => x == portal.kind))
      {
        Transision();
      }
    }

    protected override bool Handshake(Portal portal)
    {
      if (!portal)
      {
        Say("There are no available portal around :(");
        return false;
      }
      if (!portal.Handshake(this))
      {
        Say("This portal seems won't work :(");
        return false;
      }
      return true;
    }

    protected override void OnStartTransfer(Portal portal)
    {
      Say("Gotcha! This portal seems work!");
      Time.timeScale = 0.1f;
      Observable.Timer(TimeSpan.FromSeconds(_transferDurationEnter))
        .Subscribe(_ =>
        {
          Time.timeScale = 1;
          portal.ReadyTransfer(this);
          Say("Let's go beyond!");
        }).AddTo(this);
    }

    public override void OnCompleteTransfer(Portal portal)
    {
      Say("Wow, where is here...?");
      Observable.Timer(TimeSpan.FromSeconds(_transferDurationExit))
        .Subscribe(_ =>
        {
          Say("Oh, I know, I know here.");
        }).AddTo(this);
    }
  }
}
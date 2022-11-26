using System;
using System.Linq;
using UnityEngine;
using UniRx;
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

    [SerializeField] float _transferDurationEnter = 0.25f;
    [SerializeField] float _transferDurationExit = 0.25f;

    void Start()
    {
      Global.Control.Interact
          .Where(_ => nearestPortal && nearestPortal.kind == PortalKind.Door)
          .Subscribe(Transition);
    }

    public override void OnPortalEnter(Portal portal)
    {
      base.OnPortalEnter(portal);
      Say("I found out the " + portal.kind + "!");
      if (new[] { PortalKind.Passage, PortalKind.Wormhole }.Any(x => x == portal.kind))
      {
        Transition();
      }
    }

    protected override void OnStartTransfer(Portal portal)
    {
      Say("Gotcha! This portal seems work!");
      Time.timeScale = 0.1f;
      Observable.Timer(TimeSpan.FromSeconds(_transferDurationEnter))
        .Subscribe(_ =>
        {
          Time.timeScale = 1;
          Say("Let's go beyond!");
          DoneStart();
        }).AddTo(this);
    }

    public override void ProcessTransfer(Portal portal)
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
    }

    protected override void OnCompleteTransfer(Portal portal)
    {
      Say("Wow, where is here...?");
      Observable.Timer(TimeSpan.FromSeconds(_transferDurationExit))
        .Subscribe(_ =>
        {
          Say("Oh, I know, I know here.");
          DoneComplete();
        }).AddTo(this);
    }
  }
}
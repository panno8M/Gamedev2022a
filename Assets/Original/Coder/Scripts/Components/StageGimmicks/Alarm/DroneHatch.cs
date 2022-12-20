using System;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using Assembly.GameSystem.PathNetwork;

namespace Assembly.Components.Actors
{
  public class DroneHatch : PathNode
  {
    Subject<Unit> _CmdLaunch = new Subject<Unit>();
    public IObservable<Unit> CmdLaunch => _CmdLaunch;
    public PathNode nextNode => routes[0].dst;

    void Start()
    {
      AlarmMgr.Instance.IsOnAlert
        .Where(x => x)
        .Subscribe(_ => _CmdLaunch.OnNext(Unit.Default));
    }
  }
}
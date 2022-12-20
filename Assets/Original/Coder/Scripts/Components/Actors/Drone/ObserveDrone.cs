using UnityEngine;
using UniRx;

namespace Assembly.Components.Actors
{

  public class ObserveDrone : DroneAct
  {
    protected override void Subscribe()
    {
      base.Subscribe();
      OnPhaseEnter(DronePhase.Hostile)
        .Where(_ => target)
        .Subscribe(_ => AlarmMgr.Instance.ActivateAlarm()).AddTo(this);
      OnPhaseExit(DronePhase.Hostile)
        .Subscribe(_ => AlarmMgr.Instance.DisarmAlarm()).AddTo(this);
    }
  }
}
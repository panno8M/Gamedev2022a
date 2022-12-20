using UnityEngine;
using UniRx;

namespace Assembly.Components.Actors
{

  public class ObserveDrone : DroneAct
  {
    protected override void Subscribe()
    {
      base.Subscribe();
      Target
        .Where(_ => AlarmMgr.Instance)
        .Subscribe(_ =>
        {
          if (target) AlarmMgr.Instance.ActivateAlarm();
          else AlarmMgr.Instance.DisarmAlarm();
        }).AddTo(this);
    }
  }
}
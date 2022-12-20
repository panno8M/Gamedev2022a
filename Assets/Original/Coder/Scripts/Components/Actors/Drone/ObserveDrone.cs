using UnityEngine;
using UniRx;

namespace Assembly.Components.Actors
{

  public class ObserveDrone : DroneAct
  {
    protected override void Prepare()
    {
      base.Prepare();
      defaultSightRotation = sightTransform.localRotation;
    }
    protected override void Subscribe()
    {
      base.Subscribe();
      OnPhaseEnter(DronePhase.Hostile)
        .Subscribe(_ =>
        {
          AlarmMgr.Instance.ActivateAlarm();
        });
      OnPhaseExit(DronePhase.Hostile)
        .Subscribe(_ =>
        {
          AlarmMgr.Instance.DisarmAlarm();
        });
    }
  }
}
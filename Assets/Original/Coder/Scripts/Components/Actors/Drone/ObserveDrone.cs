using UniRx;

namespace Assembly.Components.Actors
{

  public class ObserveDrone : DroneAct
  {
    AlarmMgr alarmMgr;
    [Zenject.Inject]
    public void DepsInject(AlarmMgr alarmMgr)
    {
      this.alarmMgr = alarmMgr;
    }
    protected override void Subscribe()
    {
      base.Subscribe();
      aim.Target
        .Where(_ => alarmMgr)
        .Select(target => target != null)
        .Subscribe(alarmMgr.SwitchAlarm)
        .AddTo(this);
    }
  }
}
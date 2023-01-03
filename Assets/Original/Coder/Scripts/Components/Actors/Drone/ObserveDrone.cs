using UniRx;
using Assembly.Components.Pools;
using Assembly.Components.StageGimmicks;

namespace Assembly.Components.Actors
{

  public class ObserveDrone : DroneAct
  {
    AlarmMgr alarmMgr;
    [Zenject.Inject]
    public void DepsInject(
      AlarmMgr alarmMgr,
      ParticleExplosionPool psExplosionPool)
    {
      this.alarmMgr = alarmMgr;
      base.DepsInject(psExplosionPool);
    }
    protected override void Subscribe()
    {
      base.Subscribe();
      aim.Target
        .Where(_ => alarmMgr)
        .Subscribe(target => alarmMgr.SwitchAlarm(target))
        .AddTo(this);
    }
  }
}
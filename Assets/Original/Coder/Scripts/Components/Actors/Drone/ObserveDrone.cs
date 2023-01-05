using UniRx;
using Assembly.GameSystem.ObjectPool;
using Assembly.GameSystem.PathNetwork;
using Assembly.Components.Pools;
using Assembly.Components.StageGimmicks;

namespace Assembly.Components.Actors
{
  public class ObserveDrone : DroneAct
  {
    public class CreateInfo : ObjectCreateInfo<ObserveDrone>
    {
      public PathNode baseNode;
      public ParticleExplosionPool psExplosionPool;
      public AlarmMgr alarmMgr;
      public override void Infuse(ObserveDrone instance)
      {
        base.Infuse(instance);
        instance.DepsInject(
          alarmMgr: alarmMgr,
          psExplosionPool: psExplosionPool);
        instance.launcher.baseNode = baseNode;
      }
    }
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
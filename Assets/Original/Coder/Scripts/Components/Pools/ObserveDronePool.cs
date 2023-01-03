using Assembly.GameSystem;
using Assembly.GameSystem.ObjectPool;
using Assembly.GameSystem.PathNetwork;
using Assembly.Components.Actors;
using Assembly.Components.StageGimmicks;

namespace Assembly.Components.Pools
{
  public class ObserveDronePool : GameObjectPool<ObserveDrone>
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

    protected override void Blueprint()
    {
    }
    protected override ObserveDrone CreateInstance()
    {
      return prefab.Instantiate<ObserveDrone>();
    }
  }
}
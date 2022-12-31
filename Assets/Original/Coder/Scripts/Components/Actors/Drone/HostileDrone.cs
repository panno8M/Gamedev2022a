using UnityEngine;
using Assembly.Components.Pools;

namespace Assembly.Components.Actors
{

  [RequireComponent(typeof(WaterEmitterModule))]
  public class HostileDrone : DroneAct
  {
    public static HostileDronePool pool => PoolCore.Instance.hostileDrone;
    public WaterEmitterModule emitter;

    protected override void Prepare()
    {
      base.Prepare();
      emitter.Initialize();
    }
    public override void Despawn()
    {
      pool.Despawn(this);
    }
  }
}
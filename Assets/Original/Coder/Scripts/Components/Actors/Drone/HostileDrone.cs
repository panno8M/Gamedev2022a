using UnityEngine;

namespace Assembly.Components.Actors
{

  [RequireComponent(typeof(WaterEmitterModule))]
  public class HostileDrone : DroneAct
  {
    public WaterEmitterModule emitter;

    protected override void Prepare()
    {
      base.Prepare();
      emitter.Initialize();
    }
  }
}
using UnityEngine;
using UniRx;

namespace Assembly.Components.Actors
{

  [RequireComponent(typeof(WaterEmitterModule))]
  public class HostileDrone : DroneAct
  {
    public WaterEmitterModule emitter;

    public Transform hoseTransform;
    protected Quaternion defaultHoseRotation;


    protected override void Prepare()
    {
      base.Prepare();

      defaultHoseRotation = hoseTransform.localRotation;
      emitter.Initialize();
    }

    protected override void OnLostTarget()
    {
      hoseTransform.localRotation = defaultHoseRotation;
    }

    protected override void WhileLockTarget()
    {
      hoseTransform.LookAt(target);
    }
  }
}
using UnityEngine;
using UniRx;
using Assembly.GameSystem;
using Assembly.GameSystem.ObjectPool;
using Assembly.Components.Effects;
using Assembly.Components.Pools;
using Utilities;

namespace Assembly.Components.Actors
{
  public class WaterEmitterModule : DiBehavior
  {
    [SerializeField] DroneAct _actor;
    [SerializeField] Transform emitterTransform;
    [SerializeField] Transform hoseRootTransform;
    [SerializeField] float power;
    [SerializeField] EzLerp launchCoolDown = new EzLerp(3, EzLerp.Mode.Decrease);
    ObjectCreateInfo _info;

    protected Quaternion defaultHoseRootRotation;

    protected override void Blueprint()
    {
      defaultHoseRootRotation = hoseRootTransform.localRotation;

      _info = new ObjectCreateInfo
      {
        userData = emitterTransform,
      };
      _actor.ActivateSwitch(targets: this,
        cond: DronePhase.Hostile);

      _actor.CameraUpdate(this)
        .Where(_ => _actor.aim.target)
        .Subscribe(_ =>
        {
          hoseRootTransform.LookAt(_actor.aim.target.center);
          Launch();
        });
      _actor.aim.Target.Where(target => !target)
        .Subscribe(_ =>
        {
          hoseRootTransform.localRotation = defaultHoseRootRotation;
        });

    }

    public void Launch()
    {
      if (launchCoolDown.UpdFactor() == 0)
      {
        WaterBall result = Pool.waterBall.Spawn(_info);
        result.rigidbody?.AddForce(emitterTransform.forward * power, ForceMode.Acceleration);
        launchCoolDown.SetFactor1();
      }
    }
  }
}
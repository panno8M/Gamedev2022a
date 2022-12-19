using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Assembly.Components.Effects;
using Assembly.GameSystem;
using Assembly.GameSystem.ObjectPool;
using Utilities;

namespace Assembly.Components.Actors
{
  public class WaterEmitterModule : DiBehavior
  {
    [SerializeField] DroneAct _actor;
    [SerializeField] Transform emitterTransform;
    [SerializeField] float power;
    [SerializeField] EzLerp launchCoolDown = new EzLerp(3, EzLerp.Mode.Decrease);
    ObjectCreateInfo _info;

    protected override void Blueprint()
    {
      _info = new ObjectCreateInfo
      {
        userData = emitterTransform,
      };
      _actor.OnPhaseEnter(DronePhase.Hostile)
        .Subscribe(_ => enabled = true);
      _actor.OnPhaseExit(DronePhase.Hostile)
        .Subscribe(_ => enabled = false);

      this.FixedUpdateAsObservable()
        .Where(_ => _actor.target)
        .Subscribe(_ => Launch());
    }

    public void Launch()
    {
      if (launchCoolDown.UpdFactor() == 0)
      {
        WaterBall result = Pool.WaterBall.Spawn(_info);
        result.rigidbody?.AddForce(emitterTransform.forward * power, ForceMode.Acceleration);
        launchCoolDown.SetFactor1();
      }
    }
  }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assembly.GameSystem.ObjectPool;
using Utilities;

namespace Assembly.Components.Actors
{
  public class WaterEmitterModule : ActorBehavior<HostileDrone>
  {
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
    }

    public void Launch()
    {
      if (launchCoolDown == 0)
      {
        WaterBall result = WaterBallPool.Instance.Spawn(_info);
        result.rigidbody?.AddForce(emitterTransform.forward * power, ForceMode.Acceleration);
        launchCoolDown.BasisAlpha = 1;
      }
    }

    void FixedUpdate()
    {
      if (_actor.aim.target)
      {
        Launch();
      }
    }

  }
}
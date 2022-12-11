using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assembly.GameSystem.ObjectPool;
using Utilities;

namespace Assembly.Components.Actors
{
  public class WaterEmitter : MonoBehaviour
  {
    [SerializeField] Transform emitterTransform;
    [SerializeField] float power;
    [SerializeField] EzLerp launchCoolDown = new EzLerp(3, EzLerp.Mode.Decrease);
    ObjectCreateInfo _info;

    void Start()
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

  }
}
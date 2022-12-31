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
    [Zenject.Inject]
    public void DepsInject(WaterBallPool pool, ParticleImpactSplashPool psImpactSplashPool)
    {
      _waterBallCI.pool = pool;
      _waterBallCI.psImpactSplashPool = psImpactSplashPool;
    }

    [SerializeField] DroneAct _actor;
    [SerializeField] Transform emitterTransform;
    [SerializeField] Transform hoseRootTransform;
    [SerializeField] float power;
    [SerializeField] EzLerp launchCoolDown = new EzLerp(3, EzLerp.Mode.Decrease);
    WaterBallPool.CreateInfo _waterBallCI = new WaterBallPool.CreateInfo
    {
      transformUsageInfo = new TransformUsageInfo
      {
        spawnSpace = eopSpawnSpace.Global,
        referenceUsage = eopReferenceUsage.Global,
      },
      transformInfo = new TransformInfo { },
    };

    protected Quaternion defaultHoseRootRotation;

    protected override void Blueprint()
    {
      defaultHoseRootRotation = hoseRootTransform.localRotation;

      _waterBallCI.transformInfo.reference = emitterTransform;

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
        WaterBall result = _waterBallCI.pool.Spawn(_waterBallCI);
        result.rigidbody?.AddForce(emitterTransform.forward * power, ForceMode.Acceleration);
        launchCoolDown.SetFactor1();
      }
    }
  }
}
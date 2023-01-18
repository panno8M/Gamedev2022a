using System;
using UnityEngine;
using Cinemachine;
using UniRx;
using UniRx.Triggers;
using Assembly.GameSystem;
using Assembly.GameSystem.ObjectPool;
using Assembly.GameSystem.Damage;
using Assembly.Components.Pools;
using Assembly.Params;
using Utilities;

namespace Assembly.Components.Items
{
  public class Bomb : DiBehavior, IPoolCollectable
  {
    public class CreateInfo : ObjectCreateInfo<Bomb>
    {
      public ParticleExplosionPool psExplosionPool;
      public override void Infuse(Bomb instance)
      {
        base.Infuse(instance);
        instance.DepsInject(psExplosionPool);
      }
    }
    public BombParam param;

    public IDespawnable despawnable { private get; set; }
    public IObservable<Unit> OnExplode => _OnExplode;
    ParticleExplosionPool psExplosionPool;

    [Zenject.Inject]
    public void DepsInject(ParticleExplosionPool psExplosionPool)
    {
      this.psExplosionPool = psExplosionPool;
    }

    [SerializeField] ParticleSystem _psBurnUp;
    [SerializeField] DamagableComponent _damagable;
    [SerializeField] Holdable _holdable;
    [SerializeField] CinemachineImpulseSource impulseSource;
    [SerializeField] SpriteRenderer spriteRenderer;
    Subject<Unit> _OnExplode = new Subject<Unit>();

    PoolManagedParticle.CreateInfo _psExplCI = new PoolManagedParticle.CreateInfo
    {
      transformUsage = new TransformUsage
      {
        spawnSpace = eopSpawnSpace.Global,
        referenceUsage = eopReferenceUsage.Global,
      },
      transformInfo = new TransformInfo { },
    };
    Material material;
    EzLerp explosionProgress;

    public void Assemble()
    {
      _psBurnUp.Stop();
      SetHoldable(locked: false);
      SetBurnUp(burning: false);
      OnHold(holding: false);
      _OnExplode.Dispose();
      _OnExplode = new Subject<Unit>();
      material.SetFloat("_Fac", 0);
    }
    public void Disassemble()
    {
      ResetRigidbody();
      _damagable.Repair();
      _OnExplode.Dispose();
      explosionProgress.SetAsDecrease();
      explosionProgress.SetFactor0();
    }

    void OnDestroy()
    {
      if (material) { Destroy(material); }
    }

    protected override void Blueprint()
    {
      _psExplCI.transformInfo.reference = transform;
      explosionProgress = new EzLerp(param.timeToExplodeFromBroken.Seconds - param.timeToBurnUpFromBroken.Seconds);
      material = spriteRenderer.material;

      _damagable.OnBroken
        .Subscribe(_ => SetHoldable(locked: true))
        .AddTo(this);

      _damagable.OnBroken
          .Delay(param.timeToBurnUpFromBroken)
          .Subscribe(_ => SetBurnUp(burning: true))
          .AddTo(this);

      _damagable.OnBroken
          .Delay(param.timeToExplodeFromBroken)
          .Subscribe(_ =>
          {
            SetBurnUp(burning: false);
            psExplosionPool.Spawn(_psExplCI, TimeSpan.FromSeconds(1));
            impulseSource.GenerateImpulse();
            _OnExplode.OnNext(Unit.Default);
            _OnExplode.OnCompleted();
            despawnable.Despawn();
          })
          .AddTo(this);

      _holdable.OnHold.Subscribe(_ => OnHold(holding: true));
      _holdable.OnRelease.Subscribe(_ => OnHold(holding: false));
      _holdable.OnRelease
          .Delay(TimeSpan.FromMilliseconds(100))
          .Subscribe(_ => ResetRigidbody());

      this.FixedUpdateAsObservable()
        .Where(explosionProgress.isNeedsCalc)
        .Select(explosionProgress.UpdFactor)
        .Subscribe(fac =>
        {
          material.SetFloat("_Fac", param.exprodeColorCurve.Evaluate(fac));
        });
    }
    void ResetRigidbody()
    {
      rigidbody.velocity = Vector3.zero;
      rigidbody.angularVelocity = Vector3.zero;

    }
    void OnHold(bool holding)
    {
      rigidbody.useGravity = !holding;
      rigidbody.isKinematic = holding;
    }

    void SetHoldable(bool locked)
    {
      if (locked) { _holdable.ReleaseMe(); }
      _holdable.enabled = !locked;
    }
    void SetBurnUp(bool burning)
    {
      if (burning)
      {
        _psBurnUp.Play();
        explosionProgress.SetAsIncrease();
      }
      else
      { _psBurnUp.Stop(); }
    }
  }
}
using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using Assembly.GameSystem;
using Assembly.GameSystem.Damage;
using Assembly.GameSystem.ObjectPool;
using Assembly.Components.Pools;
using Assembly.Params;

namespace Assembly.Components.Actors
{
  [RequireComponent(typeof(FollowObjectModule))]
  [RequireComponent(typeof(PatrolPathModule))]
  [RequireComponent(typeof(LaunchModule))]
  [RequireComponent(typeof(AimModule))]
  public abstract class DroneAct : DiBehavior, IPoolCollectable
  {
    public IDespawnable despawnable { get; set; }
    public ParticleExplosionPool psExplosionPool { get; private set; }

    protected void DepsInject(ParticleExplosionPool psExplosionPool)
    {
      this.psExplosionPool = psExplosionPool;
    }

    Subject<Unit> _BehaviorUpdate = new Subject<Unit>();
    Subject<Unit> _CameraUpdate = new Subject<Unit>();
    Subject<Unit> _OnAssemble = new Subject<Unit>();

    Vector3 subjectiveMoveDelta;
    Vector3 objectiveMoveDelta;
    bool subjectiveMoveDeltaChanged;
    bool objectiveMoveDeltaChanged;

    PoolManagedParticle.CreateInfo _psExplCI = new PoolManagedParticle.CreateInfo
    {
      transformUsage = new TransformUsage
      {
        spawnSpace = eopSpawnSpace.Global,
        referenceUsage = eopReferenceUsage.Global,
      },
      transformInfo = new TransformInfo { },
    };

    [SerializeField] DronePhaseUnit _phase = new DronePhaseUnit();
    [SerializeField] ParticleSystem psBurnUp;

    public DroneParam param;
    public LaunchModule launcher;
    public AimModule aim;
    public FollowObjectModule follow;
    public PatrolPathModule patrol;
    public ReactionModule reaction;
    public DamagableComponent damagable;

    public IObservable<Unit> BehaviorUpdate(Behaviour x) => _BehaviorUpdate.Where(_ => x.enabled);
    public IObservable<Unit> CameraUpdate(Behaviour x) => _CameraUpdate.Where(_ => x.enabled);
    public IObservable<Unit> OnAssemble => _OnAssemble;

    public float sqrDistance(Transform target) => (target.position - transform.position).sqrMagnitude;

    public DronePhaseUnit phase => _phase;
    public void MoveSubjective(Vector3 delta)
    {
      subjectiveMoveDelta += delta;
      subjectiveMoveDeltaChanged = true;
    }
    public void MoveObjective(Vector3 delta)
    {
      objectiveMoveDelta += delta;
      objectiveMoveDeltaChanged = true;
    }

    public bool LookTowards(Transform target, float delta)
    {
      Quaternion rotation = Quaternion.LookRotation(target.position - transform.position);

      if (transform.rotation == rotation) { return true; }

      var angles = rotation.eulerAngles;
      rotation = Quaternion.Euler(
        param.constraints.ClampAngle(angles.x),
        angles.y,
        angles.z);

      if (transform.rotation == rotation) { return true; }

      transform.rotation = Quaternion.RotateTowards(
        transform.rotation,
        rotation,
        delta);
      return false;
    }
    public bool LookTowards(Transform target)
    {
      return LookTowards(target, param.settings.rotateSpeed);
    }


    void Start() { Initialize(); }
    public void Assemble()
    {
      rigidbody.isKinematic = false;
      _OnAssemble.OnNext(Unit.Default);
    }
    public void Disassemble()
    {
      rigidbody.isKinematic = true;
      phase.ShiftSleep();
    }

    protected virtual void Prepare()
    {
      _psExplCI.transformInfo.reference = transform;

      follow.Initialize();
      patrol.Initialize();
      reaction.Initialize();
      launcher.Initialize();
      aim.Initialize();
    }
    protected virtual void Subscribe()
    {
      this.FixedUpdateAsObservable()
        .Where(_ => isActiveAndEnabled)
        .Subscribe(_ =>
        {
          ApplyObjectiveMove();
          ApplySubjectiveMove();
          _BehaviorUpdate.OnNext(_);
          _CameraUpdate.OnNext(_);
        });
      damagable.OnBroken.Subscribe(_ => OnDead().Forget()).AddTo(this);

      BehaviorUpdate(this)
        .Where(phase.IsDead)
        .Subscribe(_ => rigidbody.AddForce(param.settings.gravity, ForceMode.Acceleration));

      BehaviorUpdate(this)
        .Where(phase.IsStandby)
        .Subscribe(_ =>
        {
          if (aim.target)
          { phase.ShiftHostile(); }
          else if (aim.sight.inSight)
          { phase.ShiftAttention(); }
          else if (patrol.next)
          { phase.ShiftPatrol(); }
        });
    }

    protected sealed override void Blueprint()
    {
      Prepare();
      Subscribe();

      Assemble();
    }
    void ApplySubjectiveMove()
    {
      if (!subjectiveMoveDeltaChanged) { return; }
      rigidbody.AddForce(transform.rotation * subjectiveMoveDelta * param.settings.moveSpeed - rigidbody.velocity, ForceMode.Acceleration);
      subjectiveMoveDelta = Vector3.zero;
      subjectiveMoveDeltaChanged = false;
    }
    void ApplyObjectiveMove()
    {
      if (!objectiveMoveDeltaChanged) { return; }
      rigidbody.AddForce(objectiveMoveDelta * param.settings.moveSpeed - rigidbody.velocity, ForceMode.Acceleration);
      objectiveMoveDelta = Vector3.zero;
      objectiveMoveDeltaChanged = false;
    }

    async UniTask OnDead()
    {
      psBurnUp.Play();
      phase.ShiftDead();
      await UniTask.Delay(1500);
      psExplosionPool.Spawn(_psExplCI,
        timeToDespawn: TimeSpan.FromSeconds(3));
      despawnable.Despawn();
    }
  }
}
using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using Assembly.GameSystem;
using Assembly.GameSystem.Damage;
using Assembly.GameSystem.ObjectPool;
using Assembly.Components.Pools;

namespace Assembly.Components.Actors
{
  [Flags]
  public enum DronePhase
  {
    Disactive = 0,
    Standby = 1 << 0,
    Dead = 1 << 1,

    Launch = 1 << 2,
    Patrol = 1 << 3,
    Hostile = 1 << 4,
    Attention = 1 << 5,
  }
  [RequireComponent(typeof(FollowObjectModule))]
  [RequireComponent(typeof(PatrolPathModule))]
  [RequireComponent(typeof(LaunchModule))]
  [RequireComponent(typeof(AimModule))]
  public abstract class DroneAct : DiBehavior, IPoolCollectable
  {
    public ParticleExplosionPool psExplosionPool;
    [Zenject.Inject]
    public virtual void DepsInject(ParticleExplosionPool psExplosionPool)
    {
      this.psExplosionPool = psExplosionPool;
    }

    Subject<Unit> _BehaviorUpdate = new Subject<Unit>();
    Subject<Unit> _CameraUpdate = new Subject<Unit>();
    Subject<Unit> _OnAssemble = new Subject<Unit>();

    DronePhase _previousPhase;

    Vector3 subjectiveMoveDelta;
    Vector3 objectiveMoveDelta;
    bool subjectiveMoveDeltaChanged;
    bool objectiveMoveDeltaChanged;

    ParticlePool.CreateInfo _psExplCI = new ParticlePool.CreateInfo
    {
      transformUsageInfo = new TransformUsageInfo
      {
        spawnSpace = eopSpawnSpace.Global,
        referenceUsage = eopReferenceUsage.Global,
      },
      transformInfo = new TransformInfo { },
    };

    [SerializeField] ReactiveProperty<DronePhase> _phase = new ReactiveProperty<DronePhase>();
    [SerializeField] float _gravity = -3f;
    [SerializeField] ParticleSystem psBurnUp;

    public DronePositionConstraints positionConstraints = new DronePositionConstraints
    {
      closestDistance = 3,
      furthestDistance = 4,
      relativeHeightFromGround = 1,
      speedNormal = 1,
    };
    public DroneRotationConstraints rotationConstraints = new DroneRotationConstraints
    {
      maximumHullTilt = 30,
      speedFactor = 50,
    };

    public LaunchModule launcher;
    public AimModule aim;
    public FollowObjectModule follow;
    public PatrolPathModule patrol;
    public ReactionModule reaction;
    public DamagableComponent damagable;

    public IObservable<DronePhase> OnPhaseChanged => _phase;
    public DronePhase previousPhase => _previousPhase;

    public IObservable<Unit> BehaviorUpdate(Behaviour x) => _BehaviorUpdate.Where(_ => x.enabled);
    public IObservable<Unit> CameraUpdate(Behaviour x) => _CameraUpdate.Where(_ => x.enabled);
    public IObservable<Unit> OnAssemble => _OnAssemble;

    public float sqrDistance(Transform target) => (target.position - transform.position).sqrMagnitude;

    public DronePhase phase
    {
      get { return _phase.Value; }
      private set
      {
        _previousPhase = _phase.Value;
        _phase.Value = value;
      }
    }
    public void ShiftStandby() { phase = DronePhase.Standby; }
    public void ShiftDisactive() { phase = DronePhase.Disactive; }
    public void ShiftLaunch() { phase = DronePhase.Launch; }
    public void ShiftPatrol() { phase = DronePhase.Patrol; }
    public void ShiftHostile() { phase = DronePhase.Hostile; }
    public void ShiftAttention() { phase = DronePhase.Attention; }


    public bool phaseDisactive => phase == DronePhase.Disactive;
    /// <summary>
    /// DronePhase.Disactive | DronePhase.Dead
    /// </summary>
    /// <returns></returns>
    public bool phaseSilence => (phase & (DronePhase.Disactive | DronePhase.Dead)) != 0;

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
        Mathf.Clamp((angles.x > 180f ? angles.x - 360f : angles.x), -rotationConstraints.maximumHullTilt, rotationConstraints.maximumHullTilt),
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
      return LookTowards(target, rotationConstraints.speed);
    }

    public IObservable<DronePhase> OnPhaseEnter(DronePhase phase)
    {
      return OnPhaseChanged.Where(x => phase.HasFlag(x));
    }
    public IObservable<DronePhase> OnPhaseExit(DronePhase phase)
    {
      return OnPhaseChanged.Where(x => phase.HasFlag(previousPhase));
    }

    public void ActivateSwitch(DronePhase cond, params Behaviour[] targets)
    {
      OnPhaseChanged
        .Subscribe(newPhase =>
        {
          bool b = (newPhase & cond) != 0;
          for (int i = 0; i < targets.Length; i++)
            if (targets[i])
            { targets[i].enabled = b; }
        });
    }

    void Start() { Initialize(); }
    public void Assemble()
    {
      _OnAssemble.OnNext(Unit.Default);
    }
    public void Disassemble()
    {
      ShiftDisactive();
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
        .Where(_ => phase == DronePhase.Dead)
        .Subscribe(_ => AddGravity());

      BehaviorUpdate(this)
        .Where(_ => phase == DronePhase.Attention)
        .Subscribe(_ =>
        {
          MoveObjective(Vector3.zero);
          if (aim.target || !aim.sight.inSight)
          { ShiftStandby(); }
        });

      OnPhaseEnter(DronePhase.Standby)
        .Subscribe(_ =>
        {
          if (aim.target)
          { ShiftHostile(); }
          else if (aim.sight.inSight)
          { ShiftAttention(); }
          else if (patrol.next)
          { ShiftPatrol(); }
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
      rigidbody.AddForce(transform.rotation * subjectiveMoveDelta * positionConstraints.speed - rigidbody.velocity, ForceMode.Acceleration);
      subjectiveMoveDelta = Vector3.zero;
      subjectiveMoveDeltaChanged = false;
    }
    void ApplyObjectiveMove()
    {
      if (!objectiveMoveDeltaChanged) { return; }
      rigidbody.AddForce(objectiveMoveDelta * positionConstraints.speed - rigidbody.velocity, ForceMode.Acceleration);
      objectiveMoveDelta = Vector3.zero;
      objectiveMoveDeltaChanged = false;
    }

    void AddGravity()
    {
      rigidbody.AddForce(new Vector3(0, _gravity, 0), ForceMode.Acceleration);
    }

    async UniTask OnDead()
    {
      psBurnUp.Play();
      phase = DronePhase.Dead;
      await UniTask.Delay(1500);
      psExplosionPool.Spawn(_psExplCI,
        timeToDespawn: TimeSpan.FromSeconds(3));
      ShiftDisactive();
      gameObject.SetActive(false);
    }
    public abstract void Despawn();

  }
  [System.Serializable]
  public struct DroneRotationConstraints
  {
    public float maximumHullTilt;
    public float speedFactor;
    public float speed => speedFactor * Time.deltaTime;
  }

  [System.Serializable]
  public struct DronePositionConstraints
  {
    public float closestDistance;
    public float furthestDistance;

    public float relativeHeightFromGround;

    public float speedNormal;

    public float sqrClosestDistance => closestDistance * closestDistance;
    public float sqrFurthestDistance => furthestDistance * furthestDistance;
    public float speed => speedNormal;

    public bool HasEnoughHight(Transform transform, out RaycastHit hit)
    {
      return !Physics.Raycast(
        transform.position,
        Vector3.down,
        out hit,
        relativeHeightFromGround,
        new Layers(Layer.Stage));
    }
  }
}
using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using Assembly.GameSystem;
using Assembly.GameSystem.Damage;
using Assembly.GameSystem.ObjectPool;

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
  }
  [RequireComponent(typeof(FollowObjectModule))]
  [RequireComponent(typeof(PatrolPathModule))]
  [RequireComponent(typeof(LaunchModule))]
  [RequireComponent(typeof(AimModule))]
  public abstract class DroneAct : DiBehavior, IPoolCollectable
  {
    Subject<Unit> _BehaviorUpdate = new Subject<Unit>();
    Subject<Unit> _CameraUpdate = new Subject<Unit>();

    DronePhase _previousPhase;
    Collider physicsCollider;

    Vector3 subjectiveMoveDelta;
    Vector3 objectiveMoveDelta;
    bool subjectiveMoveDeltaChanged;
    bool objectiveMoveDeltaChanged;

    [SerializeField] ReactiveProperty<DronePhase> _phase = new ReactiveProperty<DronePhase>();
    [SerializeField] float _gravity = -3f;
    [SerializeField] ParticleSystem psBurnUp;
    [SerializeField] ParticleSystem psExplode;
    [SerializeField] GameObject explDamager;

    [SerializeField]
    public DronePositionConstraints positionConstraints = new DronePositionConstraints
    {
      closestDistance = 3,
      furthestDistance = 4,
      relativeHeightFromGround = 1,
      speedFactor = 1,
    };
    [SerializeField]
    public DroneRotationConstraints rotationConstraints = new DroneRotationConstraints
    {
      maximumHullTilt = 30,
      speedFactor = 50,
    };

    public LaunchModule launcher;
    public AimModule aim;
    public FollowObjectModule follow;
    public PatrolPathModule patrol;
    public DamagableComponent damagable;

    public IObservable<DronePhase> OnPhaseChanged => _phase;
    public DronePhase previousPhase => _previousPhase;

    public IObservable<Unit> BehaviorUpdate(Behaviour x) => _BehaviorUpdate.Where(_ => x.enabled);
    public IObservable<Unit> CameraUpdate(Behaviour x) => _CameraUpdate.Where(_ => x.enabled);

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
    public void ActivateSwitch(DronePhase cond, params Collider[] targets)
    {
      OnPhaseChanged
        .Subscribe(newPhase =>
        {
          bool b = (newPhase & cond) != 0;
          for (int i = 0; i < targets.Length; i++)
          { targets[i].enabled = b; }
        });
    }

    void Start() { Initialize(); }
    public void Assemble()
    {
    }
    public void Disassemble()
    {
      ShiftDisactive();
    }

    protected virtual void Prepare()
    {
      physicsCollider = GetComponent<Collider>();
      follow.Initialize();
      patrol.Initialize();
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

      OnPhaseEnter(DronePhase.Standby)
        .Subscribe(_ =>
        {
          if (aim.target)
          { ShiftHostile(); }
          else if (patrol.next)
          { ShiftPatrol(); }
        });

      ActivateSwitch(targets: physicsCollider,
        cond: DronePhase.Patrol | DronePhase.Hostile | DronePhase.Standby);
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
      await UniTask.Delay(1000);
      psExplode.Play();
      explDamager.SetActive(true);
      await UniTask.Delay(1000);
      ShiftDisactive();
      gameObject.SetActive(false);
    }
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

    public float speedFactor;

    public float sqrClosestDistance => closestDistance * closestDistance;
    public float sqrFurthestDistance => furthestDistance * furthestDistance;
    public float speed => speedFactor;

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
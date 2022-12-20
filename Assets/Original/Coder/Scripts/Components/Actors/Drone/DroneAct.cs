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
    Disactive,
    Standby,
    Patrol,
    Hostile,
    Dead,
  }
  [RequireComponent(typeof(FollowObjectModule))]
  [RequireComponent(typeof(PatrolPathModule))]
  [RequireComponent(typeof(LaunchModule))]
  [RequireComponent(typeof(AimModule))]
  public abstract class DroneAct : DiBehavior, IPoolCollectable
  {

    [SerializeField] ReactiveProperty<Transform> _Target = new ReactiveProperty<Transform>();
    [SerializeField] ReactiveProperty<DronePhase> _phase = new ReactiveProperty<DronePhase>();
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] ParticleSystem psBurnUp;
    [SerializeField] ParticleSystem psExplode;
    [SerializeField] GameObject explDamager;

    DronePhase _previousPhase;
    Collider physicsCollider;

    Subject<Unit> _OnAssemble = new Subject<Unit>();
    Subject<Unit> _OnDisassemble = new Subject<Unit>();

    public LaunchModule launcher;
    public AimModule aim;
    public FollowObjectModule follow;
    public PatrolPathModule patrol;
    public DamagableComponent damagable;

    public IObservable<Transform> Target => _Target;
    public IObservable<DronePhase> OnPhaseChanged => _phase;
    public DronePhase previousPhase => _previousPhase;
    public IObservable<Unit> OnAssemble => _OnAssemble;
    public IObservable<Unit> OnDisassemble => _OnDisassemble;

    public Transform target
    {
      get { return _Target.Value; }
      set
      {
        _Target.Value = value;
        if (!value) { OnLostTarget(); }
      }
    }
    public DronePhase phase
    {
      get { return _phase.Value; }
      set
      {
        _previousPhase = _phase.Value;
        _phase.Value = value;
      }
    }
    public IObservable<DronePhase> OnPhaseEnter(DronePhase phase)
    {
      return OnPhaseChanged.Where(x => phase.HasFlag(x));
    }
    public IObservable<DronePhase> OnPhaseExit(DronePhase phase)
    {
      return OnPhaseChanged.Where(x => phase.HasFlag(previousPhase));
    }

    void Start() { Initialize(); }
    public void Assemble()
    {
      _OnAssemble.OnNext(Unit.Default);
      phase = DronePhase.Standby;
    }
    public void Disassemble()
    {
      phase = DronePhase.Disactive;
      _OnDisassemble.OnNext(Unit.Default);
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
      this.OnEnableAsObservable()
        .Subscribe(_ => Assemble());
      this.OnDisableAsObservable()
        .Subscribe(_ => Disassemble());

      damagable.TotalDamage
        .Where(total => total == 1)
        .Delay(TimeSpan.FromSeconds(0.5))
        .Subscribe(_ => OnDead()).AddTo(this);
      damagable.OnBroken.Subscribe(_ => OnExplode().Forget()).AddTo(this);

      this.FixedUpdateAsObservable()
        .Subscribe(_ =>
        {
          if (target)
          {
            WhileLockTarget();
          }
        }).AddTo(this);


      OnPhaseChanged
        .Subscribe(_ =>
        {
          switch (phase)
          {
            case DronePhase.Disactive:
            case DronePhase.Standby:
              if (physicsCollider.enabled)
              {
                physicsCollider.enabled = false;
              }
              break;
            case DronePhase.Patrol:
            case DronePhase.Hostile:
              if (!physicsCollider.enabled)
              {
                physicsCollider.enabled = true;
              }
              break;
          }
        });
    }

    protected sealed override void Blueprint()
    {
      Prepare();
      Subscribe();

      Assemble();
    }

    public void Move(Vector3 UnnormalizedDirection)
    {
      transform.position += UnnormalizedDirection.normalized * moveSpeed * Time.deltaTime;
    }

    protected virtual void OnDead()
    {
      rigidbody.useGravity = true;
      rigidbody.isKinematic = false;
      psBurnUp.Play();
      phase = DronePhase.Dead;
    }
    async UniTask OnExplode()
    {
      psExplode.Play();
      explDamager.SetActive(true);
      await UniTask.Delay(1000);
      phase = DronePhase.Disactive;
      gameObject.SetActive(false);
    }
    protected virtual void OnLostTarget() { }
    protected virtual void WhileLockTarget() { }
  }
}
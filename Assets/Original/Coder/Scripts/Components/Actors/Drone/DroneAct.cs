using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using Senses.Sight;
using Assembly.GameSystem;
using Assembly.GameSystem.Damage;
using Assembly.GameSystem.ObjectPool;
using Utilities;

namespace Assembly.Components.Actors
{
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
    protected Quaternion defaultSightRotation;

    Subject<Unit> _OnAssemble = new Subject<Unit>();
    Subject<Unit> _OnDisassemble = new Subject<Unit>();
    Subject<Unit>[] _OnPhaseEnter = new Subject<Unit>[Enum.GetNames(typeof(DronePhase)).Length];
    Subject<Unit>[] _OnPhaseExit = new Subject<Unit>[Enum.GetNames(typeof(DronePhase)).Length];

    public LaunchModule launcher;
    public FollowObjectModule follow;
    public PatrolPathModule patrol;
    public AiSight sight;
    public DamagableComponent damagable;
    public Transform sightTransform;

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
        _OnPhaseExit[(int)previousPhase].OnNext(Unit.Default);
        _OnPhaseEnter[(int)phase].OnNext(Unit.Default);
      }
    }
    public IObservable<Unit> OnPhaseEnter(DronePhase phase)
    { return _OnPhaseEnter[(int)phase]; }
    public IObservable<Unit> OnPhaseExit(DronePhase phase)
    { return _OnPhaseExit[(int)phase]; }

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
      for (int i = 0; i < _OnPhaseEnter.Length; i++)
      {
        _OnPhaseEnter[i] = new Subject<Unit>();
        _OnPhaseExit[i] = new Subject<Unit>();
      }

      physicsCollider = GetComponent<Collider>();
      follow.Initialize();
      patrol.Initialize();
      launcher.Initialize();
      defaultSightRotation = sightTransform.localRotation;
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

      sight.InSight
        .Where(_ => phase != DronePhase.Disactive)
        .Subscribe(visible => target = visible ? visible.center : null)
        .AddTo(this);

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
            case DronePhase.Dead:
              sightTransform.gameObject.SetActive(false);
              break;
            case DronePhase.Patrol:
            case DronePhase.Hostile:
              if (!physicsCollider.enabled)
              {
                physicsCollider.enabled = true;
                sightTransform.gameObject.SetActive(true);
              }
              break;
          }
        });

      SwipeCamera().Forget();
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

    public float CamSwipeMin;
    public float CamSwipeMax;
    async UniTask SwipeCamera()
    {
      bool isMin = true;
      while (true)
      {
        switch (phase)
        {
          case DronePhase.Patrol:
            var deg = isMin ? CamSwipeMin : CamSwipeMax;
            var target = Quaternion.AngleAxis(deg, transform.right) * transform.rotation;
            var rot = Quaternion.RotateTowards(sightTransform.rotation, target, .1f);
            sightTransform.rotation = rot;
            if (rot == target) { isMin = !isMin; }
            break;
          case DronePhase.Hostile:
            if (this.target)
            {
              sightTransform.LookAt(this.target);
            }
            break;
        }

        do
        {
          await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
        }
        while (!this || !isActiveAndEnabled);
      }
    }

  }
}
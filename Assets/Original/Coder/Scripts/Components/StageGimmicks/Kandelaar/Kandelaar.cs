using System;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using Cinemachine;
using Assembly.GameSystem;
using Assembly.GameSystem.Damage;

namespace Assembly.Components.StageGimmicks
{
  public class Kandelaar : DiBehavior, IRollbackDispatcher
  {
    UI.SimpleFader fader;
    [SerializeField]
    Rollback rollback;
    [Zenject.Inject]
    public void DepsInject(UI.SimpleFader fader, Rollback rollback)
    {
      this.fader = fader;
      if (!this.rollback) { this.rollback = rollback; }
    }
    Vector3 _defaultPosition;
    [SerializeField] Holdable _holdable;
    [SerializeField] DamagableComponent _damagable;
    [SerializeField] ParticleSystem _psSmoke;
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] CinemachineImpulseSource impulseSource;
    public KandelaarSupply supply;

    public Holdable holdable => _holdable;

    protected override void Blueprint()
    {
      throw new NotImplementedException();
    }
    void Start()
    {
      Prepare();
      supply.Initialize();
    }
    void Prepare()
    {
      _defaultPosition = transform.position;
      _holdable.OnHold
          .Subscribe(_ =>
          {
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;
            supply.enabled = false;
          });
      _holdable.OnRelease
          .Subscribe(_ =>
          {
            rigidbody.useGravity = true;
            rigidbody.isKinematic = false;
            transform.rotation = Quaternion.identity;
            supply.enabled = true;
            Observable.Timer(TimeSpan.FromMilliseconds(100))
              .Subscribe(_ =>
              {
                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
              });
          });
      _damagable.OnBroken
        .Subscribe(_ => BreakSequence().Forget())
        .AddTo(this);
    }
    async UniTask BreakSequence()
    {
      virtualCamera.Priority = 100;
      await GameTime.HitStop(TimeSpan.FromMilliseconds(500));
      impulseSource.GenerateImpulseAt(transform.position, new Vector3(0, -1, 0));
      await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
      impulseSource.GenerateImpulseAt(transform.position, new Vector3(1, 0, 0));
      await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
      impulseSource.GenerateImpulseAt(transform.position, new Vector3(0, 0, 1));

      _psSmoke.Play();
      supply.enabled = false;
      _holdable.enabled = false;
      await UniTask.Delay(1000);
      _psSmoke.Stop();

      fader.Rollback().Forget();

      await UniTask.Delay(1000);
      await RollbackSequence();
    }
    async UniTask RollbackSequence()
    {
      _damagable.Repair();
      transform.position = _defaultPosition;
      supply.enabled = true;
      _holdable.enabled = true;
      virtualCamera.Priority = 0;
      rollback.Preflight(this);
      await UniTask.Delay(1000);
      rollback.Execute(this);
    }
  }
}

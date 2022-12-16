using System;
using UniRx;
using UnityEngine;
using Assembly.GameSystem;
using Assembly.GameSystem.Message;
using Assembly.GameSystem.Damage;
using Cysharp.Threading.Tasks;

namespace Assembly.Components.StageGimmicks
{
  public class Kandelaar : DiBehavior
  {
    Vector3 _defaultPosition;
    [SerializeField] MessageDispatcher _OnRollback = new MessageDispatcher(MessageKind.Invoke);
    [SerializeField] Holdable _holdable;
    [SerializeField] DamagableComponent _damagable;
    [SerializeField] ParticleSystem _psSmoke;
    public KandelaarSupply supply;

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
          });
      _holdable.OnRelease
          .Subscribe(_ =>
          {
            rigidbody.useGravity = true;
            rigidbody.isKinematic = false;
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
      Debug.Log("Kandelaar Broken!");
      _psSmoke.Play();
      supply.enabled = false;
      _holdable.enabled = false;
      await UniTask.Delay(1000);
      _psSmoke.Stop();
      UI.SimpleFader.Instance.progress.secDuration = 1f;
      UI.SimpleFader.Instance.progress.SetAsIncrease();

      await UniTask.Delay(1000);

      await RollbackSequence();

      await UniTask.Delay(1000);

      UI.SimpleFader.Instance.progress.SetAsDecrease();
      await UniTask.Delay(1000);

      UI.SimpleFader.Instance.progress.secDuration = .3f;
      UI.SimpleFader.Instance.progress.SetAsIncrease();
      await UniTask.Delay(300);
      UI.SimpleFader.Instance.progress.SetAsDecrease();
      await UniTask.Delay(300);


    }
    UniTask RollbackSequence()
    {
      _damagable.Repair();
      transform.position = _defaultPosition;
      supply.enabled = true;
      _holdable.enabled = true;
      _OnRollback.Dispatch();
      return UniTask.CompletedTask;
    }
  }
}

using System;
using UniRx;
using UnityEngine;
using Assembly.GameSystem;
using Assembly.GameSystem.Damage;

namespace Assembly.Components.StageGimmicks
{
  public class Kandelaar : DiBehavior
  {
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
        .Subscribe(_ =>
        {
          Debug.Log("Kandelaar Broken!");
          _psSmoke.Play();
          Observable.TimerFrame(1).Subscribe(_ => _psSmoke.Stop());
        }).AddTo(this);
    }
  }
}

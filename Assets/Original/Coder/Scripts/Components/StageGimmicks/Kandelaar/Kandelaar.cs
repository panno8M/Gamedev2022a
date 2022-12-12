using System;
using UniRx;
using UnityEngine;
using Assembly.GameSystem;
using Assembly.GameSystem.Damage;
using Assembly.Components.Actors;

namespace Assembly.Components.StageGimmicks
{
  public class Kandelaar : DiBehavior
  {
    [SerializeField] Holdable _holdable;
    [SerializeField] DamagableComponent _damagable;
    [SerializeField] ParticleSystem _psSmoke;

    [SerializeField] SafetyTrigger _supplyFieldTrigger;
    PlayerFlameReceptor _playerFlameReceptor;

    void Start()
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

      _supplyFieldTrigger.OnEnter
        .Subscribe(trigger =>
        {
          if (!_playerFlameReceptor)
          {
            _playerFlameReceptor = trigger.GetComponent<PlayerFlameReceptor>();
          }
          if (_playerFlameReceptor)
          {
            _playerFlameReceptor.flameQuantity = 1;
          }
        });
    }
  }
}

using System;
using UniRx;
using UniRx.Ex.InteractionTraits.Core;
using UnityEngine;
using Assembly.GameSystem.Damage;
using Assembly.Components.Actors;

namespace Assembly.Components.StageGimmicks
{
  public class Kandelaar : MonoBehaviour
  {
    [SerializeField] Interactable _interactable;
    [SerializeField] DamagableComponent _damagable;
    [SerializeField] ParticleSystem _psSmoke;
    [SerializeField] Rigidbody _rb;

    [SerializeField] SafetyTrigger _supplyFieldTrigger;
    PlayerFlameReceptor _playerFlameReceptor;

    void Start()
    {
      _interactable.holdable.OnHold
          .Subscribe(_ =>
          {
            _rb.useGravity = false;
            _rb.isKinematic = true;
          });
      _interactable.holdable.OnRelease
          .Subscribe(_ =>
          {
            _rb.useGravity = true;
            _rb.isKinematic = false;
            Observable.Timer(TimeSpan.FromMilliseconds(100))
              .Subscribe(_ =>
              {
                _rb.velocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
              });
          });
      _damagable.OnBroken
        .Subscribe(_ =>
        {
          Debug.Log("Kandelaar Broken!");
          _psSmoke.Play();
          Observable.TimerFrame(1).Subscribe(_ => _psSmoke.Stop());
        }).AddTo(this);

      _supplyFieldTrigger.Triggers.ObserveAdd()
        .Subscribe(trigger =>
        {
          if (!_playerFlameReceptor)
          {
            _playerFlameReceptor = trigger.Value.GetComponent<PlayerFlameReceptor>();
          }
          if (_playerFlameReceptor)
          {
            _playerFlameReceptor.flameQuantity = 1;
          }
        });
    }
  }
}

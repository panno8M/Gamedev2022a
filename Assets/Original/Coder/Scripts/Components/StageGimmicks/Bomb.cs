using System;
using UnityEngine;
using UniRx;
using Senses.Pain;
using UniRx.Ex.InteractionTraits.Core;

namespace Assembly.Components.StageGimmicks
{

  public class Bomb : MonoBehaviour
  {

    [SerializeField] ParticleSystem _psBurnUp;
    [SerializeField] ParticleSystem _psExplosion;
    [SerializeField] DamagableComponent _damagable;
    [SerializeField] Interactable _interactable;
    [SerializeField] float secExplosionDelay = 4;

    Rigidbody _rb;
    [SerializeField] SpriteRenderer _renderer;
    [SerializeField] Collider _damagerCollider;

    void Start()
    {
      _rb = GetComponent<Rigidbody>();
      _damagerCollider.enabled = false;

      _damagable.OnBroken
          .Subscribe(_ =>
          {
            _interactable.holdable.Disactivate();
          }).AddTo(this);

      _damagable.OnBroken
          .Delay(TimeSpan.FromSeconds(0.5))
          .Subscribe(_ => _psBurnUp.Play())
          .AddTo(this);

      _damagable.OnBroken
          .Delay(TimeSpan.FromSeconds(secExplosionDelay))
          .Subscribe(_ =>
          {
            Explode();
            _damagerCollider.enabled = true;
            Destroy(gameObject, 1);
          })
          .AddTo(this);

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
          });
      _interactable.holdable.OnRelease
          .Delay(TimeSpan.FromMilliseconds(100))
          .Subscribe(_ =>
          {
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
          });
    }

    void Explode()
    {
      _damagable.enabled = false;
      _renderer.enabled = false;
      GetComponent<Collider>().enabled = false;
      _psExplosion.Play();
    }
  }
}
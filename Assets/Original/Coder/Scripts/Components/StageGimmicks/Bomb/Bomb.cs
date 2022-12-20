using System;
using UnityEngine;
using UniRx;
using Assembly.GameSystem;
using Assembly.GameSystem.ObjectPool;
using Assembly.GameSystem.Damage;

namespace Assembly.Components.StageGimmicks
{

  public class Bomb : DiBehavior, IPoolCollectable
  {
    [SerializeField] ParticleSystem _psBurnUp;
    [SerializeField] ParticleSystem _psExplosion;
    [SerializeField] DamagableComponent _damagable;
    [SerializeField] Holdable _holdable;
    [SerializeField] float secExplosionDelay = 4;

    [SerializeField] SpriteRenderer _renderer;
    [SerializeField] Collider _physicsCollider;
    [SerializeField] GameObject _damager;

    public void Assemble()
    {
      _psBurnUp.Stop();
      SetHoldable(locked: false);
      SetBurnUp(burning: false);
      SetExplode(exploding: false);
      OnHold(holding: false);
    }
    public void Disassemble()
    {
      ResetRigidbody();
      _damagable.Repair();
    }

    protected override void Blueprint()
    {
      _physicsCollider = GetComponent<Collider>();

      _damagable.OnBroken
        .Subscribe(_ => SetHoldable(locked: true))
        .AddTo(this);

      _damagable.OnBroken
          .Delay(TimeSpan.FromSeconds(0.5))
          .Subscribe(_ => SetBurnUp(burning: true))
          .AddTo(this);

      _damagable.OnBroken
          .Delay(TimeSpan.FromSeconds(secExplosionDelay))
          .Subscribe(_ =>
          {
            SetBurnUp(burning: false);
            SetExplode(exploding: true);
            Observable.Timer(TimeSpan.FromSeconds(1))
              .Subscribe(_ => Pool.Bomb.Despawn(this)).AddTo(this);
          })
          .AddTo(this);

      _holdable.OnHold.Subscribe(_ => OnHold(holding: true));
      _holdable.OnRelease.Subscribe(_ => OnHold(holding: false));
      _holdable.OnRelease
          .Delay(TimeSpan.FromMilliseconds(100))
          .Subscribe(_ => ResetRigidbody());
    }
    void ResetRigidbody()
    {
      rigidbody.velocity = Vector3.zero;
      rigidbody.angularVelocity = Vector3.zero;

    }
    void OnHold(bool holding)
    {
      rigidbody.useGravity = !holding;
      rigidbody.isKinematic = holding;
    }

    void SetHoldable(bool locked)
    {
      _holdable.enabled = !locked;
    }
    void SetBurnUp(bool burning)
    {
      if (burning)
      {
        _psBurnUp.Play();
      }
      else
      {
        _psBurnUp.Stop();
      }
    }

    void SetExplode(bool exploding)
    {
      _damagable.enabled = !exploding;
      _renderer.enabled = !exploding;
      _physicsCollider.enabled = !exploding;
      _damager.SetActive(exploding);
      if (exploding)
      {
        _psExplosion.Play();
      }
      else
      {
        _psExplosion.Stop();
      }
    }
  }
}
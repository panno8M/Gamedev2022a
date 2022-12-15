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
    Vector3 _defaultPosition;

    [SerializeField] ParticleSystem _psBurnUp;
    [SerializeField] ParticleSystem _psExplosion;
    [SerializeField] DamagableComponent _damagable;
    [SerializeField] Holdable _holdable;
    [SerializeField] float secExplosionDelay = 4;

    [SerializeField] SpriteRenderer _renderer;
    [SerializeField] Collider _damagerCollider;

    public void Assemble()
    {
      transform.position = _defaultPosition;
      transform.rotation = Quaternion.identity;
      _damagerCollider.enabled = false;
      _damagable.enabled = true;
      _renderer.enabled = true;
      GetComponent<Collider>().enabled = true;
      _holdable.enabled = true;
      _damagable.Repair();
    }
    public void Disassemble() {}

    void Start()
    {
      _defaultPosition = transform.position;

      _damagerCollider.enabled = false;

      _damagable.OnBroken
          .Subscribe(_ =>
          {
            _holdable.enabled = false;
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
            Observable.Timer(TimeSpan.FromSeconds(1))
              .Subscribe(_ =>
              {
                BombPool.Instance.Despawn(this);
                _psExplosion.Stop();
              }).AddTo(this);
          })
          .AddTo(this);

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
          });
      _holdable.OnRelease
          .Delay(TimeSpan.FromMilliseconds(100))
          .Subscribe(_ =>
          {
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
          });
    }

    void Explode()
    {
      _damagable.enabled = false;
      _renderer.enabled = false;
      GetComponent<Collider>().enabled = false;
      _damagerCollider.enabled = true;
      _psExplosion.Play();
      _psBurnUp.Stop();
    }
  }
}
using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Assembly.GameSystem;
using Assembly.GameSystem.ObjectPool;

namespace Assembly.Components.Effects
{
  public class WaterBall : DiBehavior, IPoolCollectable
  {
    [SerializeField] ParticleSystem psImpactSplash;

    [SerializeField] Collider _physicsCollider;
    [SerializeField] Renderer _renderer;

    protected override void Blueprint()
    {
      this.FixedUpdateAsObservable()
      .Subscribe(_ =>
      {
        rigidbody.AddForce(new Vector3(0, -2f, 0), ForceMode.Acceleration);
      });
    }

    public void Assemble()
    {
      _physicsCollider.enabled = true;
      _renderer.enabled = true;
    }
    public void Disassemble()
    {
      rigidbody.velocity = Vector3.zero;
    }

    void OnCollisionEnter(Collision other)
    {
      psImpactSplash.Play();
      _physicsCollider.enabled = false;
      _renderer.enabled = false;
      rigidbody.velocity = Vector3.zero;
      Observable.Timer(TimeSpan.FromSeconds(3)).Subscribe(_ =>
      {
        Pool.WaterBall.Despawn(this);
      }).AddTo(this);
    }
  }
}
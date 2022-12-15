using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Assembly.GameSystem;
using Assembly.GameSystem.ObjectPool;
public class WaterBall : DiBehavior, IPoolCollectable
{
  [SerializeField] ParticleSystem psImpactSplash;

  [SerializeField] Collider _physicsCollider;
  [SerializeField] Renderer _renderer;
  public void Assemble()
  {
    _physicsCollider.enabled = true;
    _renderer.enabled = true;
  }
  public void Disassemble()
  {
    rigidbody.velocity = Vector3.zero;
  }
  void FixedUpdate()
  {
    rigidbody.AddForce(new Vector3(0, -2f, 0), ForceMode.Acceleration);
  }

  void OnCollisionEnter(Collision other)
  {
    psImpactSplash.Play();
    _physicsCollider.enabled = false;
    _renderer.enabled = false;
    rigidbody.velocity = Vector3.zero;
    Observable.Timer(TimeSpan.FromSeconds(3)).Subscribe(_ =>
    {
      WaterBallPool.Instance.Despawn(this);
    }).AddTo(this);
  }
}

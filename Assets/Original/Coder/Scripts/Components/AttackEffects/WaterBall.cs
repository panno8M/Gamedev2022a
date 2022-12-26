using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Assembly.GameSystem;
using Assembly.GameSystem.ObjectPool;
using Assembly.Components.Pools;

namespace Assembly.Components.Effects
{
  public class WaterBall : DiBehavior, IPoolCollectable
  {
    ObjectCreateInfo _info = new ObjectCreateInfo { };

    protected override void Blueprint()
    {
      this.FixedUpdateAsObservable()
      .Subscribe(_ =>
      {
        rigidbody.AddForce(new Vector3(0, -2f, 0), ForceMode.Acceleration);
      });
    }

    public void Assemble() { }
    public void Disassemble()
    {
      rigidbody.velocity = Vector3.zero;
      rigidbody.isKinematic = false;
    }

    void OnCollisionEnter(Collision other)
    {
      _info.position = transform.position;
      Pool.psImpactSplash.Spawn(_info,
        timeToDespawn: TimeSpan.FromSeconds(3f));
      Pool.waterBall.Despawn(this);
    }
  }
}
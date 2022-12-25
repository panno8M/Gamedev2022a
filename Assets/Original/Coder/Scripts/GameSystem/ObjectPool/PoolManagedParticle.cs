using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assembly.GameSystem.ObjectPool
{
  public class PoolManagedParticle : DiBehavior, IPoolCollectable
  {
    [SerializeField] ParticleSystem _particle;
    public ParticleSystem particle => _particle ?? (_particle = GetComponent<ParticleSystem>());
    protected override void Blueprint()
    {
    }
    public void Assemble()
    {
      particle.Play();
    }
    public void Disassemble()
    {
      particle.Stop();
    }
  }
}
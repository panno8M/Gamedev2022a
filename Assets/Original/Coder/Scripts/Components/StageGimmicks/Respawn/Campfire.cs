using UnityEngine;
using UniRx;
using Assembly.GameSystem.Damage;
using Assembly.Components.Pools;

namespace Assembly.Components.StageGimmicks
{
  public class Campfire : MonoBehaviour, ISpawnSpot
  {
    [SerializeField] DamagableComponent damagable;
    [SerializeField] ParticleSystem psBurnUp;

    void Start()
    {
      damagable.OnBroken.Subscribe(_ =>
      {
        Pool.player.activeSpot = this;
      });
    }
    public Vector3 spawnPosition => transform.position;
    public void OnActivate()
    {
      psBurnUp.Play();
      damagable.Break();
    }
    public void OnDeactivate()
    {
      psBurnUp.Stop();
      damagable.Repair();
    }
  }
}
using UnityEngine;
using UniRx;
using Assembly.GameSystem.Damage;
using Assembly.Components.Pools;

namespace Assembly.Components.StageGimmicks
{
  public class CheckPoint : MonoBehaviour, ISpawnSpot
  {
    PlayerPool pool;
    [Zenject.Inject]
    public void DepsInject(PlayerPool pool)
    {
      this.pool = pool;
    }

    [SerializeField] bool activateOnAwake;
    public Vector3 spawnPosition => transform.position;

    void Awake()
    {
      if (activateOnAwake)
      {
        pool.activeSpot = this;
      }
    }

    public void OnActivate()
    {
    }
    public void OnDeactivate()
    {
    }
  }
}
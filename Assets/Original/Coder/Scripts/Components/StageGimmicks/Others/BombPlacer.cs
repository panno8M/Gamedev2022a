using System;
using UnityEngine;
using UniRx;
using Assembly.GameSystem.ObjectPool;
using Assembly.Components.Pools;
using Assembly.Components.Items;

namespace Assembly.Components.StageGimmicks
{
  public class BombPlacer : MonoBehaviour
  {
    BombPool pool;
    [SerializeField]
    Rollback rollback;

    [Zenject.Inject]
    public void DepsInject(BombPool pool, ParticleExplosionPool psExplosionPool, Rollback rollback)
    {
      this.pool = pool;
      _bombCI.psExplosionPool = psExplosionPool;
      if (!this.rollback) { this.rollback = rollback; }
    }

    TransformInfo[] _bombTransformInfos;
    Bomb.CreateInfo _bombCI = new Bomb.CreateInfo
    {
      transformUsage = new TransformUsage
      {
        spawnSpace = eopSpawnSpace.Global,
        referenceUsage = eopReferenceUsage.Global,
      }
    };
    [SerializeField] Bomb[] instances;
    void Start()
    {
      _bombTransformInfos = new TransformInfo[transform.childCount];
      instances = new Bomb[transform.childCount];

      for (int i = 0; i < transform.childCount; i++)
      {
        _bombTransformInfos[i] = new TransformInfo
        {
          reference = transform.GetChild(i),
        };
        _bombCI.transformInfo = _bombTransformInfos[i];
        instances[i] = pool.Spawn(_bombCI);
      }

      rollback.OnExecute
        .Subscribe(_ =>
        {
          for (int i = 0; i < instances.Length; i++)
          {
            _bombCI.transformInfo = _bombTransformInfos[i];
            pool.Respawn(instances[i], _bombCI, TimeSpan.FromSeconds(1));
          }
        });
    }
  }
}
using UnityEngine;
using UniRx;
using Assembly.GameSystem.ObjectPool;
using Assembly.Components.Pools;
using Assembly.Components.Items;
using Assembly.Params;

namespace Assembly.Components.StageGimmicks
{
  public class BombPlacer : MonoBehaviour
  {
    public BombPlacerParam param;
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
        SpawnBomb(i);
      }

      rollback.OnPreflight
        .Subscribe(_ =>
        {
          for (int i = 0; i < instances.Length; i++)
          { pool.Despawn(instances[i]); }
        });

      rollback.OnExecute
        .Subscribe(_ =>
        {
          for (int i = 0; i < instances.Length; i++)
          { SpawnBomb(i); }
        });
    }
    void SpawnBomb(int index)
    {
      _bombCI.transformInfo = _bombTransformInfos[index];
      instances[index] = pool.Spawn(_bombCI);
      instances[index].OnExplode
        .Delay(param.timeToRespawnFromDespawn)
        .Where(_ => instances[index] && !instances[index].isActiveAndEnabled)
        .Subscribe(_ => SpawnBomb(index));
    }
  }
}
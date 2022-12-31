using System;
using UnityEngine;
using Assembly.GameSystem.ObjectPool;
using Assembly.GameSystem.Message;
using Assembly.Components.Pools;

namespace Assembly.Components.StageGimmicks
{
  public class BombPlacer : MonoBehaviour, IMessageListener
  {
    BombPool pool;

    [Zenject.Inject]
    public void DepsInject(BombPool pool, ParticleExplosionPool psExplosionPool)
    {
      this.pool = pool;
      _bombCI.pool = pool;
      _bombCI.psExplosionPool = psExplosionPool;

    }

    TransformInfo[] _bombTransformInfos;
    BombPool.CreateInfo _bombCI = new BombPool.CreateInfo
    {
      transformUsageInfo = new TransformUsageInfo
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
    }
    public void ReceiveMessage(MessageUnit message)
    {
      switch (message.kind)
      {
        case MessageKind.Invoke:
          for (int i = 0; i < instances.Length; i++)
          {
            _bombCI.transformInfo = _bombTransformInfos[i];
            pool.Respawn(instances[i], _bombCI, TimeSpan.FromSeconds(1));
          }
          break;

      }
    }
  }
}
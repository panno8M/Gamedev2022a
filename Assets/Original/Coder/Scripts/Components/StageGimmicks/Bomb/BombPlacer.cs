using System;
using UnityEngine;
using UniRx;
using Assembly.GameSystem.ObjectPool;
using Assembly.GameSystem.Message;
using Assembly.Components.Pools;

namespace Assembly.Components.StageGimmicks
{
  public class BombPlacer : MonoBehaviour, IMessageListener
  {
    BombPool.CreateInfo[] _bombCIs;
    [SerializeField] Bomb[] instances;
    void Start()
    {
      _bombCIs = new BombPool.CreateInfo[transform.childCount];
      instances = new Bomb[transform.childCount];

      for (int i = 0; i < transform.childCount; i++)
      {

        _bombCIs[i] = new BombPool.CreateInfo
        {
          spawnSpace = eopSpawnSpace.Global,
          referenceUsage = eopReferenceUsage.Global,
          reference = transform.GetChild(i),
        };
        instances[i] = Pool.bomb.Spawn(_bombCIs[i]);
      }
    }
    public void ReceiveMessage(MessageUnit message)
    {
      switch (message.kind)
      {
        case MessageKind.Invoke:
          for (int i = 0; i < instances.Length; i++)
          {
            Pool.bomb.Respawn(instances[i], _bombCIs[i], TimeSpan.FromSeconds(1));
          }
          break;

      }
    }
  }
}
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
    ObjectCreateInfo[] infos;
    [SerializeField] Bomb[] instances;
    void Start()
    {
      infos = new ObjectCreateInfo[transform.childCount];
      instances = new Bomb[transform.childCount];

      for (int i = 0; i < transform.childCount; i++)
      {

        infos[i] = new ObjectCreateInfo
        {
          offset = transform.GetChild(i)
        };
        instances[i] = Pool.bomb.Spawn(infos[i]);
      }
    }
    public void ReceiveMessage(MessageUnit message)
    {
      switch (message.kind)
      {
        case MessageKind.Invoke:
          for (int i = 0; i < instances.Length; i++)
          {
            Pool.bomb.Respawn(instances[i], infos[i], TimeSpan.FromSeconds(1));
          }
          break;

      }
    }
  }
}
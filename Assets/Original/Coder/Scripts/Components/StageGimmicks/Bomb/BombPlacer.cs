using System.Collections.Generic;
using UnityEngine;
using Assembly.GameSystem.ObjectPool;
using Assembly.GameSystem.Message;
using Assembly.Components.Pools;

namespace Assembly.Components.StageGimmicks
{
  public class BombPlacer : MonoBehaviour, IMessageListener
  {
    List<ObjectCreateInfo> infos;
    List<Bomb> instances;
    void Start()
    {
      infos = new List<ObjectCreateInfo>(transform.childCount);
      instances = new List<Bomb>(transform.childCount);

      for (int i = 0; i < transform.childCount; i++)
      {

        infos.Add(new ObjectCreateInfo
        {
          userData = transform.GetChild(i)
        });
        instances.Add(Pool.bomb.Spawn(infos[i]));
      }
    }
    public void ReceiveMessage(MessageUnit message)
    {
      switch (message.kind)
      {
        case MessageKind.Invoke:
          for (int i = 0; i < instances.Count; i++)
          {
            Pool.bomb.Respawn(instances[i], infos[i]);
          }
          break;

      }
    }
  }
}
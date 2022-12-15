using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Assembly.GameSystem.ObjectPool;

namespace Assembly.Components.StageGimmicks
{
  public class BombPlacer : MonoBehaviour
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
        instances.Add(BombPool.Instance.Spawn(infos[i]));
      }
      (BombPool.Instance as BombPool).OnClear.Subscribe(_ =>
      {
        for (int i = 0; i < instances.Count; i++)
          BombPool.Instance.Respawn(instances[i], infos[i]);
      }
      );
    }
  }
}
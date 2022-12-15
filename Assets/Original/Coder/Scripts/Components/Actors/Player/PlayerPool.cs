using UnityEngine;
using Assembly.GameSystem.ObjectPool;

namespace Assembly.Components.Actors
{
  public class PlayerPool : GameObjectPool<PlayerAct>
  {
    public Transform activeSpawnPoint;
    PlayerAct _player;
    public PlayerAct player => _player ?? Spawn();

    protected override PlayerAct CreateInstance()
    {
      if (_player == null)
      {
        _player = (PlayerAct)FindObjectOfType(typeof(PlayerAct));

        if (_player == null)
        {
          var result = GameObject.Instantiate(prefab, activeSpawnPoint.position, Quaternion.identity);
          result.name = prefab.name;
          _player = result.GetComponent<PlayerAct>();
        }
      }

      return _player;
    }
    protected override void InfuseInfoOnSpawn(PlayerAct newObj, ObjectCreateInfo info)
    {
    }

    public void Despawn() { Despawn(_player); }

    protected override void Blueprint()
    {
      Spawn();
    }
  }
}
using UnityEngine;
using UniRx;
using Assembly.GameSystem.ObjectPool;
using Assembly.GameSystem.Message;

namespace Assembly.Components.Actors.Player
{
  public class PlayerPool : GameObjectPool<PlayerAct>
  {
    public Transform activeSpawnPoint;
    PlayerAct _player;
    public PlayerAct player => _player ?? Spawn();
    [SerializeField] MessageDispatcher _OnRespawn = new MessageDispatcher(MessageKind.Invoke);

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
      OnSpawn.Subscribe(_ => _OnRespawn.Dispatch());
    }

    void OnDrawGizmos()
    {
      _OnRespawn.DrawArrow(transform);
    }
  }
}
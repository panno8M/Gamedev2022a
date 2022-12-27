using System;
using UnityEngine;
using UniRx;
using Assembly.GameSystem;
using Assembly.GameSystem.ObjectPool;
using Assembly.GameSystem.Message;

using Assembly.Components.StageGimmicks;
using Assembly.Components.Actors.Player;

namespace Assembly.Components.Pools
{
  public class PlayerPool : GameObjectPool<PlayerAct>
  {
    PlayerAct _player;
    public PlayerAct player => _player ?? Spawn();
    [SerializeField] MessageDispatcher _OnRespawn = new MessageDispatcher(MessageKind.Invoke);


    [SerializeField] CheckPoint defaultSpot;
    ReactiveProperty<ISpawnSpot> ActiveSpot = new ReactiveProperty<ISpawnSpot>();
    public IObservable<ISpawnSpot> OnActiveSpotChanged => ActiveSpot;
    public ISpawnSpot activeSpot
    {
      get { return ActiveSpot.Value ?? defaultSpot; }
      set
      {
        if (activeSpot == value) { return; }
        DeactivateCurrentSpot(currentSpot: activeSpot);
        ActivateNewSpot(newSpot: value);
        ActiveSpot.Value = value;
      }
    }
    void DeactivateCurrentSpot(ISpawnSpot currentSpot)
    {
      currentSpot.Deactivate();
    }
    void ActivateNewSpot(ISpawnSpot newSpot)
    {
      newSpot.Activate();
    }

    protected override PlayerAct CreateInstance()
    {
      if (_player == null)
      {
        _player = (PlayerAct)FindObjectOfType(typeof(PlayerAct));

        if (_player == null)
        {
          _player = prefab.Instantiate<PlayerAct>();
          _player.name = prefab.name;
        }
      }

      return _player;
    }
    protected override void InfuseInfoOnSpawn(PlayerAct newObj, ObjectCreateInfo info)
    {
      newObj.transform.position = activeSpot.spawnPosition;
    }

    public void Despawn() { Despawn(_player); }

    protected override void Blueprint()
    {
      Spawn();
      OnSpawn().Subscribe(_ => _OnRespawn.Dispatch());
    }

    void OnDrawGizmos()
    {
      _OnRespawn.DrawArrow(transform);
    }
  }
}
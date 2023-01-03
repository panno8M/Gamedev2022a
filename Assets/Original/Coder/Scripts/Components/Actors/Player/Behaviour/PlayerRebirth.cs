using System;
using UniRx;

using Assembly.Components.StageGimmicks;

namespace Assembly.Components.Actors.Player
{
  public class PlayerRebirth : ActorBehavior<PlayerAct>
  {
    ReactiveProperty<ISpawnSpot> ActiveSpot = new ReactiveProperty<ISpawnSpot>();
    public IObservable<ISpawnSpot> OnActiveSpotChanged => ActiveSpot;
    public ISpawnSpot activeSpot
    {
      get => ActiveSpot.Value;
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

    public void Spawn()
    {
      if (_actor.isActiveAndEnabled) { return; }
      _actor.gameObject.SetActive(true);
      _actor.Assemble();
      _actor.transform.position = activeSpot.spawnPosition;
    }
    public void Despawn()
    {
      if (!_actor.isActiveAndEnabled) { return; }
      _actor.Disassemble();
      _actor.gameObject.SetActive(false);
    }
    public void Respawn() { Despawn(); Spawn(); }

    protected override void Blueprint() { }
  }
}
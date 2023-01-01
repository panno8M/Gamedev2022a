using UnityEngine;
using UniRx;
using Assembly.GameSystem.Damage;
using Assembly.Components.Actors.Player;

namespace Assembly.Components.StageGimmicks
{
  public class Campfire : MonoBehaviour, ISpawnSpot
  {
    PlayerAct player;
    [Zenject.Inject]
    public void DepsInject(PlayerAct player)
    {
      this.player = player;
    }

    [SerializeField] DamagableComponent damagable;
    [SerializeField] ParticleSystem psBurnUp;

    void Start()
    {
      damagable.OnBroken.Subscribe(_ =>
      {
        player.rebirth.activeSpot = this;
      });
    }
    public Vector3 spawnPosition => transform.position;
    public void OnActivate()
    {
      psBurnUp.Play();
      damagable.Break();
    }
    public void OnDeactivate()
    {
      psBurnUp.Stop();
      damagable.Repair();
    }
  }
}
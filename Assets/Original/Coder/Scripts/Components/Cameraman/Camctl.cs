using UnityEngine;
using Cinemachine;
using Assembly.GameSystem;
using Assembly.Components.Actors.Player;

namespace Assembly.Components
{
  public class Camctl : DiBehavior
  {
    void Start() => InitializeAfter(player);

    PlayerAct player;
    [Zenject.Inject]
    public void DepsInject(PlayerAct player)
    {
      this.player = player;
    }
    [SerializeField] CinemachineVirtualCamera cmDefault;
    protected override void Blueprint()
    {
      cmDefault.m_Follow = player.transform;
    }
  }
}

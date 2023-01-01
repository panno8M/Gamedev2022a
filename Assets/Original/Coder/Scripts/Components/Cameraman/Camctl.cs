using UnityEngine;
using Cinemachine;
using UniRx;
using Assembly.GameSystem;
using Assembly.Components.Actors.Player;

namespace Assembly.Components
{
  public class Camctl : DiBehavior
  {
    PlayerAct player;
    [Zenject.Inject]
    public void DepsInject(PlayerAct player)
    {
      this.player = player;
    }
    [SerializeField] CinemachineVirtualCamera cmDefault;
    protected override void Blueprint()
    {
      throw new System.NotImplementedException();
    }

    void Awake()
    {
      player.OnAssembleObservable
          .Subscribe(_ =>
          {
            cmDefault.m_Follow = player.transform;
          }).AddTo(this);

    }

  }
}

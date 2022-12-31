using UnityEngine;
using Cinemachine;
using UniRx;
using Assembly.GameSystem;
using Assembly.Components.Pools;

namespace Assembly.Components
{
  public class Camctl : DiBehavior
  {
    PlayerPool playerPool;
    [Zenject.Inject]
    public void DepsInject(PlayerPool playerPool)
    {
      this.playerPool = playerPool;
    }
    [SerializeField] CinemachineVirtualCamera cmDefault;
    protected override void Blueprint()
    {
      throw new System.NotImplementedException();
    }

    void Awake()
    {
      playerPool.OnSpawn()
          .Subscribe(player =>
          {
            cmDefault.m_Follow = player.transform;
          }).AddTo(this);

    }

  }
}

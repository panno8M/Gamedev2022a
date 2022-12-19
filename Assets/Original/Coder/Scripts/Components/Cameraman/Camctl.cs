using UnityEngine;
using Cinemachine;
using UniRx;
using Assembly.GameSystem;

namespace Assembly.Components
{
  public class Camctl : UniqueBehaviour<Camctl>
  {
    [SerializeField] CinemachineVirtualCamera cmDefault;
    protected override void Blueprint()
    {
      throw new System.NotImplementedException();
    }

    void Awake()
    {
      Pool.Player?.OnSpawn
          .Subscribe(player =>
          {
            cmDefault.m_Follow = player.transform;
          }).AddTo(this);

    }

  }
}

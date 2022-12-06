using UnityEngine;
using Cinemachine;
using UniRx;
using Assembly.GameSystem;

namespace Assembly.Components
{
  public class Camctl : UniqueBehaviour<Camctl>
  {
    [SerializeField] CinemachineVirtualCamera cmDefault;

    void Awake()
    {
      Global.PlayerPool?.OnSpawn
          .Subscribe(player =>
          {
            cmDefault.m_Follow = player.transform;
          }).AddTo(this);

    }

  }
}

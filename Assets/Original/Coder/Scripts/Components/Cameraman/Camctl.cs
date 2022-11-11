using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UniRx;

namespace Assembly.Components
{
  public class Camctl : UniqueBehaviour<Camctl>
  {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init() { instance = null; }

    [SerializeField] CinemachineVirtualCamera cmDefault;

    void Awake()
    {
      Global.PlayerRespawn?.OnRent
          .Subscribe(player =>
          {
            cmDefault.m_Follow = player.transform;
          }).AddTo(this);

    }

  }
}

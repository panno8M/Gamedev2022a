using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assembly.GameSystem.Message;

namespace Assembly.Components.StageGimmicks
{
  public class FlamePillar : MonoBehaviour, IMessageReceiver
  {
    ParticleSystem ps;
    bool active;
    void Start()
    {
      ps = GetComponent<ParticleSystem>();
      ps.Stop();
    }
    public void ReceiveMessage(MessageUnit message)
    {
      if (!active && message.signalPower != 0)
      {
        active = true;
        ps.Play();
      }
      else if (active && message.signalPower == 0)
      {
        active = false;
        ps.Stop();
      }
    }

  }
}
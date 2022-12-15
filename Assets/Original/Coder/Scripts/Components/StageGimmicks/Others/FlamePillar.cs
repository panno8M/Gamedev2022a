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
      switch (message.kind)
      {
        case MessageKind.Signal:
          if (!active && message.intensity.factor != 0)
          {
            active = true;
            ps.Play();
          }
          else if (active && message.intensity.factor == 0)
          {
            active = false;
            ps.Stop();
          }
          break;
      }
    }

  }
}
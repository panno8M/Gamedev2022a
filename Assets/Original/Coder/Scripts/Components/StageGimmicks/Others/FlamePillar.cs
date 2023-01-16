using UnityEngine;
using Assembly.GameSystem.Message;
using Utilities;

namespace Assembly.Components.StageGimmicks
{
  public class FlamePillar : MonoBehaviour, IMessageListener
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
          if (message.intensity.UpdFactor() == 0)
          {
            if (active)
            {
              active = false;
              ps.Stop();
            }
          }
          else
          {
            if (!active)
            {
              active = true;
              ps.Play();
            }
          }
          break;
      }
    }
    public void Powered(MixFactor powerGain) { }

  }
}
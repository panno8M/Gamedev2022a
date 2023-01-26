using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assembly.GameSystem.Message;
using Utilities;

namespace Assembly.Components.StageGimmicks
{
  public class LightMapChange : MonoBehaviour, IMessageListener
  {
    [SerializeField] LightShift lightShift;
    [SerializeField] int indexWhenOff = 0;
    [SerializeField] int indexWhenOn = 1;

    public void Powered(MixFactor powerGain)
    {
      if (powerGain.PeekFactor() == 0)
      {
        lightShift.Set(indexWhenOff);
      }
      else if (powerGain.PeekFactor() == 1)
      {
        lightShift.Set(indexWhenOn);
      }
    }

    public void ReceiveSignal(MixFactor message)
    {

    }
  }
}
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

        public void Powered(MixFactor powerGain)
        {
            if(powerGain.PeekFactor() == 0 && lightShift.lastIndex != powerGain.PeekFactor()){
                lightShift.Set(0);
            }else{
                if(powerGain.PeekFactor() == 1 && lightShift.lastIndex != powerGain.PeekFactor()){
                    lightShift.Set(1);
                }
            }
        }

        public void ReceiveSignal(MixFactor message)
        {
            
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using Assembly.GameSystem.Message;
using UniRx;
using System;

namespace Assembly.Components.StageGimmicks{
[RequireComponent(typeof(SafetyTrigger))]
    public class Lever : MonoBehaviour
    {   
        SafetyTrigger safetyTrigger;
        // enum Switch { Off = -1, On = 1 }
        // Switch targetSwitch = Switch.Off;
        bool switchs;
        [SerializeField] MessageDispatcher _OnSwitch = new MessageDispatcher();
        EzLerp messageSwitch = new EzLerp(1);
        [SerializeField]MixFactor mixFactor;

        // Start is called before the first frame update
        void Start()
        {
            safetyTrigger = GetComponent<SafetyTrigger>();
            //_OnSwitch.message.intensity = mixFactor.;
        }

        // Update is called once per frame
        void Update()
        {
            _OnSwitch.Dispatch();
            //Debug.Log(safetyTrigger.triggers[0]);
        }
    }
}
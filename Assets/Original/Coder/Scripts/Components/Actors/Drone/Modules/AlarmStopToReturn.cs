using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using UniRx.Triggers;
using UniRx;

namespace Assembly.Components.Actors{
    public class AlarmStopToReturn : MonoBehaviour
    {
        public DroneHatch droneHatch;
        [SerializeField] Renderer droneRenderer;
        bool isVisible;

        void Start(){
            droneRenderer.OnBecameInvisibleAsObservable().Subscribe(_ => OnLeftCamera());
        }

        void OnLeftCamera(){
            if(!droneHatch.alarmMgr.IsAlarm.Value){
                droneHatch.Reset();
            }
        }
    }
}
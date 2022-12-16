using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assembly.GameSystem.ObjectPool;
using Utilities;

namespace Assembly.Components.Actors{
    public class AlarmUser : ActorCore<ObserveDrone>
    {
        ObjectCreateInfo _info;

        public AlarmMgr alarmMgr; 

        public void AlarmUse()
        {
            if (alarmMgr.alarmRemainingTime.UpdFactor() == 0)
            {
                alarmMgr.IsAlarm.Value = true;
            }
        }

        void FixedUpdate()
        {
            if (_actor.aim.target)
            {
                AlarmUse();
            }
        }
    }
}

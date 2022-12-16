using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Utilities;

public class AlarmMgr : MonoBehaviour
{
    public ReactiveProperty<bool> IsAlarm;
    //public BoolReactiveProperty IsAlarm = new BoolReactiveProperty();
    public EzLerp alarmRemainingTime = new EzLerp(1, EzLerp.Mode.Increase);

    void Start(){
        IsAlarm.Where(x => x==true).Subscribe(_ => AlarmStart());
    }

    void AlarmStart(){
        alarmRemainingTime.SetFactor1();
        Debug.Log("ALARM!!!");
    }

    
}

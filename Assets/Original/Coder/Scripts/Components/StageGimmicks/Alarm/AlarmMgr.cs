using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Utilities;

public class AlarmMgr : MonoBehaviour
{
    public ReactiveProperty<bool> IsAlarm;
    //public BoolReactiveProperty IsAlarm = new BoolReactiveProperty();
    public ReactiveProperty<float> alarmRemainingTime;
    public float alarmLength;

    void Start(){
        IsAlarm.Where(x => x==true).Subscribe(_ => AlarmStart());
    }

    void AlarmStart(){
        alarmRemainingTime.Value = alarmLength;
        Debug.Log("ALARM!!!");
    }

    void FixedUpdate(){
        if(alarmRemainingTime.Value > 0){
            alarmRemainingTime.Value = alarmRemainingTime.Value - 1 * Time.deltaTime;
        } else{
            IsAlarm.Value = false;
        }
    }
    
}

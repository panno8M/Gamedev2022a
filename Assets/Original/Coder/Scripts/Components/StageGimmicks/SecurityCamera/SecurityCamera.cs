using UnityEngine;
using UniRx;
using Senses.Sight;

public class SecurityCamera : MonoBehaviour
{

    [SerializeField]AiSight aiSight;
    Vector3ReactiveProperty targetTransform;
    SafetyTrigger safetyTrigger;

    void Start(){
        safetyTrigger = aiSight.gameObject.GetComponent<SafetyTrigger>();
        aiSight.InSight.Subscribe(x => OnAlarm(x)).AddTo(this);
    }

    void OnAlarm(AiVisible target){
        if (target){
            AlarmMgr.Instance.ActivateAlarm();
        }else{
            AlarmMgr.Instance.DisarmAlarm();
        }
    }
    
}

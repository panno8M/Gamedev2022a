using UnityEngine;
using UniRx;
using Senses.Sight;

namespace Assembly.Components.StageGimmicks
{
  public class SecurityCamera : MonoBehaviour
  {
    AlarmMgr alarmMgr;
    [Zenject.Inject]
    public void DepsInject(AlarmMgr alarmMgr)
    {
      this.alarmMgr = alarmMgr;
    }

    [SerializeField] AiSight aiSight;

    void Start()
    {
      aiSight.Noticed
        .Subscribe(x => alarmMgr.SwitchAlarm(x))
        .AddTo(this);
    }
  }
}
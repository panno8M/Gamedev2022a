using UnityEngine;
using UniRx;
using Senses.Sight;
using Assembly.GameSystem;
using Assembly.GameSystem.Message;

namespace Assembly.Components.StageGimmicks
{
  public class SecurityCamera : MonoBehaviour, IMessageListener
  {
    public bool inverseSignal;
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

    public void ReceiveMessage(MessageUnit message)
    {
      switch (message.kind)
      {
        case MessageKind.Signal:
        case MessageKind.Invoke:
          if (message.intensity.PeekFactor() == 0)
          { aiSight.SetActiveOnce(inverseSignal); }
          if (message.intensity.PeekFactor() == 1)
          { aiSight.SetActiveOnce(!inverseSignal); }
          break;
      }
    }
  }
}
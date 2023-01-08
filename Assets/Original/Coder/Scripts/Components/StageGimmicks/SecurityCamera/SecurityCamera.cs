using UnityEngine;
using UniRx;
using Senses.Sight;
using Assembly.GameSystem;
using Assembly.GameSystem.Message;
using Assembly.GameSystem.Damage;

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
    [SerializeField] DamagableComponent damagable;

    void Start()
    {
      aiSight.Noticed
        .Subscribe(x => alarmMgr.SwitchAlarm(x))
        .AddTo(this);

      damagable.OnBroken
        .Subscribe(_ => Destroy(gameObject));
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
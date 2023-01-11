using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using Senses.Sight;
using Assembly.GameSystem;
using Assembly.GameSystem.Message;
using Assembly.GameSystem.Damage;
using Assembly.Params;

namespace Assembly.Components.StageGimmicks
{
  public class SecurityCamera : MonoBehaviour, IMessageListener
  {
    public BlinkParam blinkParam;
    public bool inverseSignal;
    AlarmMgr alarmMgr;
    [Zenject.Inject]
    public void DepsInject(AlarmMgr alarmMgr)
    {
      this.alarmMgr = alarmMgr;
    }

    [SerializeField] AiSight aiSight;
    [SerializeField] DamagableComponent damagable;

    bool _poweron;
    bool _blinking;
    bool poweron
    {
      get => _poweron;
      set
      {
        _poweron = value;
        aiSight.SetActiveOnce(poweron && !blinking);
      }
    }
    bool blinking
    {
      get => _blinking;
      set
      {
        _blinking = value;
        aiSight.SetActiveOnce(poweron && !blinking);
      }
    }

    void Start()
    {
      aiSight.Noticed
        .Subscribe(x => alarmMgr.SwitchAlarm(x))
        .AddTo(this);

      damagable.OnBroken
        .Subscribe(_ => Destroy(gameObject));

      if (blinkParam.useBlink) Blink().Forget();
    }
    async UniTask Blink()
    {
      while (true)
      {
        blinking = false;
        await UniTask.Delay(blinkParam.secondsToWait.PickRandomMilliSeconds());
        blinking = true;
        await UniTask.Delay(blinkParam.secondsToBlink.PickRandomMilliSeconds());
        blinking = false;
      }
    }

    public void ReceiveMessage(MessageUnit message)
    {
      switch (message.kind)
      {
        case MessageKind.Signal:
        case MessageKind.Invoke:
          if (message.intensity.PeekFactor() == 0)
          { poweron = inverseSignal; }
          if (message.intensity.PeekFactor() == 1)
          { poweron = !inverseSignal; }
          break;
      }
    }
  }
}
#if UNITY_EDITOR
// #define DEBUG_SECURITY_CAMERA
#endif

using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using Senses.Sight;
using Assembly.GameSystem;
using Assembly.GameSystem.Message;
using Assembly.GameSystem.Damage;
using Assembly.Params;
using Utilities;

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

#if DEBUG_SECURITY_CAMERA
    [SerializeField]
#endif
    bool _signalOn;
#if DEBUG_SECURITY_CAMERA
    [SerializeField]
#endif
    bool _powerOn;
#if DEBUG_SECURITY_CAMERA
    [SerializeField]
#endif
    bool _blinking;

    bool isSightActivable => _signalOn && _powerOn && !_blinking;
    bool signalOn
    {
      get => _signalOn;
      set { _signalOn = value; aiSight.SetActiveOnce(isSightActivable); }
    }
    bool powerOn
    {
      get => _powerOn;
      set { _powerOn = value; aiSight.SetActiveOnce(isSightActivable); }
    }
    bool blinking
    {
      get => _blinking;
      set { _blinking = value; aiSight.SetActiveOnce(isSightActivable); }
    }

    void Start()
    {
      signalOn = inverseSignal;

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
        if (!this) { return; }

        if (signalOn && powerOn) { blinking = true; }

        await UniTask.Delay(blinkParam.secondsToBlink.PickRandomMilliSeconds());
        if (!this) { return; }

        blinking = false;
      }
    }

    public void ReceiveSignal(MixFactor signal)
    {
      if (signal.PeekFactor() == 0)
      { signalOn = !inverseSignal; }
      if (signal.PeekFactor() == 1)
      { signalOn = inverseSignal; }
    }
    public void Powered(MixFactor powerGain)
    {
      powerOn = powerGain.PeekFactor() == 1;
    }
  }
}
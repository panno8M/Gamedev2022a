using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using Utilities;
using Assembly.GameSystem.Message;

namespace Assembly.Components.StageGimmicks
{
  [RequireComponent(typeof(SafetyTrigger))]
  [RequireComponent(typeof(SignalLineDrawer))]
  public class PressurePlate : MonoBehaviour
  {
    SafetyTrigger _trigger;
    SignalLineDrawer signalLineDrawer;

    enum Mode { Relax = -1, Press = 1 }
    [SerializeField] MessageDispatcher _OnPress = new MessageDispatcher();
    Mode targetMode = Mode.Relax;
    [SerializeField]
    EzLerp animateProgress = new EzLerp(1);

    [SerializeField] GameObject _plateObject;
    Material _plateMaterial;
    Vector3 _positionDefault;
    [SerializeField] Vector3 _positionDelta;
    Color _relaxColor;
    [SerializeField] Color _pressColor;


    void Start()
    {
      _trigger = GetComponent<SafetyTrigger>();
      (signalLineDrawer = GetComponent<SignalLineDrawer>()).Initialize();
      signalLineDrawer.dispatchers.Add(_OnPress);
      _positionDefault = _plateObject.transform.localPosition;
      _plateMaterial = _plateObject.GetComponent<Renderer>().material;
      _relaxColor = _plateMaterial.color;

      _OnPress.message.intensity = animateProgress;

      AnimatePress().Forget();

      _trigger.OnEnter
        .Subscribe(trigger =>
        {
          targetMode = Mode.Press;
        }).AddTo(this);
      _trigger.OnExit
        .Subscribe(trigger =>
        {
          if (this._trigger.others.Count == 0)
          {
            targetMode = Mode.Relax;
          }
        }).AddTo(this);
    }
    void OnDestroy()
    {
      if (_plateMaterial) { Destroy(_plateMaterial); }
    }

    void CalcFrame()
    {
      animateProgress.mode = (EzLerp.Mode)targetMode;
      if (animateProgress.needsCalc)
      {
        _OnPress.Dispatch();

        _plateObject.transform.localPosition = animateProgress.UpdAdd(_positionDefault, _positionDelta);
        if (_plateMaterial)
        {
          _plateMaterial.color = animateProgress.UpdMix(_relaxColor, _pressColor);
        }
      }

    }
    async UniTask AnimatePress()
    {
      while (Application.isPlaying)
      {
        CalcFrame();
        await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
      }
      _plateObject.transform.localPosition = _positionDefault;
    }

    void OnDrawGizmos()
    {
      _OnPress.DrawArrow(transform);
    }
  }
}
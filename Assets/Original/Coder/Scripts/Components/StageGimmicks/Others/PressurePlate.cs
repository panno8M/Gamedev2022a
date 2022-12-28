using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using Utilities;
using Assembly.GameSystem.Message;

namespace Assembly.Components.StageGimmicks
{
  [RequireComponent(typeof(SafetyTrigger))]
  public class PressurePlate : MonoBehaviour
  {
    SafetyTrigger SafetyTrigger;

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
      SafetyTrigger = GetComponent<SafetyTrigger>();
      _positionDefault = _plateObject.transform.localPosition;
      _plateMaterial = _plateObject.GetComponent<Renderer>().material;
      _relaxColor = _plateMaterial.color;

      _OnPress.message.intensity = animateProgress;

      AnimatePress().Forget();
      // this.FixedUpdateAsObservable()
      //   .Subscribe(_ => CalcFrame());

      SafetyTrigger.OnEnter
        .Subscribe(trigger =>
        {
          targetMode = Mode.Press;
        }).AddTo(this);
      SafetyTrigger.OnExit
        .Subscribe(trigger =>
        {
          if (SafetyTrigger.triggers.Count == 0)
          {
            targetMode = Mode.Relax;
          }
        }).AddTo(this);
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
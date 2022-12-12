using UnityEngine;
using UniRx;
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
      AnimatePress().Forget();

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

    async UniTask AnimatePress()
    {
      while (Application.isPlaying)
      {
        animateProgress.mode = (EzLerp.Mode)targetMode;
        if (_OnPress.message.signalPower != animateProgress)
        {
          _OnPress.message.signalPower = animateProgress;
          _OnPress.Dispatch();

          _plateObject.transform.localPosition = animateProgress.Add(_positionDefault, _positionDelta);
          if (_plateMaterial)
          {
            _plateMaterial.color = animateProgress.Mix(_relaxColor, _pressColor);
          }
        }

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
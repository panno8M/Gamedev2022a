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
    bool isPressed;
    [SerializeField]
    int presscount;
    bool isToggleFlipped;
    SafetyTrigger _trigger;

    enum Mode { Relax = -1, Press = 1 }
    [SerializeField] MessageDispatcher _OnPress = new MessageDispatcher();
    [SerializeField] MessageDispatcher _OnSwitch = new MessageDispatcher();
    [SerializeField] MessageDispatcher _OnSwitchInv = new MessageDispatcher();
    [SerializeField][Range(0, 1)] float pressThrethold = .5f;

    Mode targetMode = Mode.Relax;
    [SerializeField]
    EzLerp animateProgress = new EzLerp(1);
    [SerializeField]
    EzLerp switchProgress = new EzLerp(1);

    [SerializeField] GameObject _plateObject;
    Material _plateMaterial;
    Vector3 _positionDefault;
    [SerializeField] Vector3 _positionDelta;
    Color _relaxColor;
    [SerializeField] Color _pressColor;


    void Start()
    {
      _trigger = GetComponent<SafetyTrigger>();
      _positionDefault = _plateObject.transform.localPosition;
      _plateMaterial = _plateObject.GetComponent<Renderer>().material;
      _relaxColor = _plateMaterial.color;

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
    MixFactor __inv = new MixFactor();
    void CalcFrame()
    {
      animateProgress.mode = (EzLerp.Mode)targetMode;
      if (animateProgress.needsCalc)
      {
        _plateObject.transform.localPosition = animateProgress.UpdAdd(_positionDefault, _positionDelta);
        if (_plateMaterial)
        {
          _plateMaterial.color = animateProgress.Mix(_relaxColor, _pressColor);
        }

        bool isPressedPrev = isPressed;
        isPressed = (animateProgress.PeekFactor() >= pressThrethold);
        if (isPressed && !isPressedPrev)
        {
          presscount++;
          switchProgress.SetMode(presscount % 2 == 1);
        }
        _OnPress.Dispatch(animateProgress);
      }
      if (switchProgress.needsCalc)
      {
        switchProgress.UpdFactor();
        __inv.SetFactor(switchProgress.Invpeek());

        _OnSwitch.Dispatch(switchProgress);
        _OnSwitchInv.Dispatch(__inv);
      }

    }
    async UniTask AnimatePress()
    {
      while (true)
      {
        if (!this || !_plateObject) { return; }
        CalcFrame();
        await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
      }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
      _OnPress.DrawArrow(transform, nameof(_OnPress));
      _OnSwitch.DrawArrow(transform, nameof(_OnSwitch));
      _OnSwitchInv.DrawArrow(transform, nameof(_OnSwitchInv));
    }
#endif
  }
}
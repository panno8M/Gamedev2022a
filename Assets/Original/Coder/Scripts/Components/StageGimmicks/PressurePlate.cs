using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;

namespace Assembly.Components.StageGimmicks
{
  [RequireComponent(typeof(SafetyTrigger))]
  public class PressurePlate : MonoBehaviour
  {
    SafetyTrigger SafetyTrigger;

    enum Mode { Relax = -1, Press = 1 }
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

      SafetyTrigger.Triggers.ObserveAdd()
        .Subscribe(trigger =>
        {
          targetMode = Mode.Press;
        }).AddTo(this);
      SafetyTrigger.Triggers.ObserveRemove()
        .Subscribe(trigger =>
        {
          if (SafetyTrigger.Triggers.Count == 0)
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
        _plateObject.transform.localPosition = animateProgress.Add(_positionDefault, _positionDelta);
        if (_plateMaterial)
        {
          _plateMaterial.color = animateProgress.Mix(_relaxColor, _pressColor);
        }
        await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
      }
      _plateObject.transform.localPosition = _positionDefault;
    }
  }
}
#if UNITY_EDITOR
// #define DEBUG_LIFT
#endif

using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Assembly.GameSystem.Message;
using Utilities;

namespace Assembly.Components.StageGimmicks
{
  public class Lift : MonoBehaviour, IMessageListener
  {
#if DEBUG_LIFT
    [Header("[Debug Inspector]\ndon't forget to turn symbol DEBUG_LIFT off.")]
    byte __headerTarget__;
#endif
    enum OperationMode
    {
      FollowIntensity = 1 << 0,
      PingPong = 1 << 1,
    }
    [SerializeField] OperationMode mode;
    [SerializeField] bool ignorePower;
    [SerializeField] Vector3 _positionDelta;

    [SerializeField] GameObject _plateObject;
    [SerializeField] Color _acitivatedColor;
    [SerializeField] EzLerp animateProgress;

#if DEBUG_LIFT
    [Header("Debug")]
#endif
#if DEBUG_LIFT
    [SerializeField]
#endif
    float timescalePower = 1;
#if DEBUG_LIFT
    [SerializeField]
#endif
    float timescaleSignal = 0;


    Vector3 _positionDefault;
    Material _plateMaterial;
    Color _relaxColor;

    void Start()
    {
      _positionDefault = transform.localPosition;
      _plateMaterial = _plateObject.GetComponent<Renderer>().material;
      _relaxColor = _plateMaterial.color;

      animateProgress.localTimeScale = timescalePower * timescaleSignal;

      this.FixedUpdateAsObservable()
        .Where(animateProgress.isNeedsCalc)
        .Subscribe(_ => UpdatePosition(transform, animateProgress));

      animateProgress.NeedsCalc
        .Where(x => mode == OperationMode.PingPong && !x)
        .Subscribe(animateProgress.FlipMode);
    }

    public void ReceiveMessage(MessageUnit message)
    {
      switch (message.kind)
      {
        case MessageKind.Signal:
          switch (mode)
          {
            case OperationMode.FollowIntensity:
              UpdatePosition(transform, message.intensity);
              break;
            case OperationMode.PingPong:
              timescaleSignal = message.intensity.PeekFactor();
              break;
          }
          break;
        case MessageKind.Power:
          if (ignorePower) { break; }
          timescalePower = message.intensity.PeekFactor();
          _plateMaterial.color = message.intensity.Mix(_relaxColor, _acitivatedColor);
          break;
      }
      animateProgress.localTimeScale = timescaleSignal * timescalePower;
    }

    void UpdatePosition(Transform transform, MixFactor intensity)
    {
      transform.localPosition = intensity.UpdAdd(_positionDefault, _positionDelta);
    }
  }
}

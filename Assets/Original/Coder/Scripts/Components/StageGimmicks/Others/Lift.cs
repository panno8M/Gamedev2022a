using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Assembly.GameSystem.Message;
using Utilities;

namespace Assembly.Components.StageGimmicks
{
  public class Lift : MonoBehaviour, IMessageListener
  {
    enum OperationMode
    {
      FollowIntensity = 1 << 0,
      PingPong = 1 << 1,
    }
    [SerializeField] OperationMode mode;
    [SerializeField] bool ignorePower;
    [SerializeField][Range(0, 1)] float _power;
    [SerializeField] Vector3 _positionDelta;

    [SerializeField] GameObject _plateObject;
    [SerializeField] Color _acitivatedColor;
    [SerializeField] EzLerp animateProgress;

    Vector3 _positionDefault;
    Material _plateMaterial;
    Color _relaxColor;

    new Transform transform => _plateObject.transform;
    float power => ignorePower ? 1 : _power;
    void Start()
    {
      _positionDefault = transform.localPosition;
      _plateMaterial = _plateObject.GetComponent<Renderer>().material;
      _relaxColor = _plateMaterial.color;

      this.FixedUpdateAsObservable()
        .Where(_ => mode == OperationMode.PingPong)
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
          if (mode == OperationMode.FollowIntensity)
          { UpdatePosition(transform, message.intensity); }
          else
          { animateProgress.localTimeScale = message.intensity.PeekFactor(); }
          break;
        case MessageKind.Power:
          _power = message.intensity.PeekFactor();
          _plateMaterial.color = message.intensity.Mix(_relaxColor, _acitivatedColor);
          break;
      }
    }

    void UpdatePosition(Transform transform, MixFactor intensity)
    {
      transform.localPosition = intensity.UpdAdd(_positionDefault, _positionDelta * power);
    }
  }
}

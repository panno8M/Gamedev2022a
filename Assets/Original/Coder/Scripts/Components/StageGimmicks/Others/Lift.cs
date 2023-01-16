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
    [SerializeField] bool inverseSignal;
    [SerializeField] Vector3 _positionDelta;
    Vector3 positionDelta => transform.localRotation * _positionDelta;

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

    void UpdateTimeScale()
      => animateProgress.localTimeScale = inverseSignal
        ? timescalePower * (1 - timescaleSignal)
        : timescalePower * timescaleSignal;

    void Start()
    {
      _positionDefault = _plateObject.transform.localPosition;
      _plateMaterial = _plateObject.GetComponent<Renderer>().material;
      _relaxColor = _plateMaterial.color;

      UpdateTimeScale();

      this.FixedUpdateAsObservable()
        .Where(animateProgress.isNeedsCalc)
        .Subscribe(_ => UpdatePosition(_plateObject.transform, animateProgress));

      animateProgress.NeedsCalc
        .Where(x => mode == OperationMode.PingPong && !x)
        .Subscribe(animateProgress.FlipMode);
    }
    void OnDestroy()
    {
      if (_plateMaterial) { Destroy(_plateMaterial); }
    }

    public void ReceiveMessage(MessageUnit message)
    {
      switch (message.kind)
      {
        case MessageKind.Signal:
          switch (mode)
          {
            case OperationMode.FollowIntensity:
              UpdatePosition(_plateObject.transform, message.intensity);
              break;
            case OperationMode.PingPong:
              timescaleSignal = message.intensity.PeekFactor();
              break;
          }
          break;
      }
      UpdateTimeScale();
    }
    public void Powered(MixFactor powerGain)
    {
      timescalePower = powerGain.PeekFactor();
      _plateMaterial.color = powerGain.Mix(_relaxColor, _acitivatedColor);
      UpdateTimeScale();
    }

    void UpdatePosition(Transform transform, MixFactor intensity)
    {
      transform.localPosition = intensity.UpdAdd(_positionDefault, positionDelta);
    }

#if UNITY_EDITOR
    Mesh _plateMesh;
    Vector3 _positionDefaultGlobal;
    void OnDrawGizmos()
    {
      Vector3 positionDelta = this.positionDelta;

      Gizmos.color = Color.white;
      if (!Application.isPlaying || _positionDefaultGlobal == Vector3.zero)
      { _positionDefaultGlobal = _plateObject.transform.position; }
      Gizmos.DrawLine(_positionDefaultGlobal, _positionDefaultGlobal + positionDelta);
      if (_plateObject)
      {
        if (!_plateMesh) { _plateMesh = _plateObject.GetComponent<MeshFilter>()?.sharedMesh; }
        if (_plateMesh)
        {
          Gizmos.DrawMesh(_plateMesh, _positionDefaultGlobal + positionDelta, _plateObject.transform.rotation, _plateObject.transform.localScale);
        }
      }
    }
#endif
  }
}

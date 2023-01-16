#if UNITY_EDITOR
// #define DEBUG_TRANSFORMER
#endif

using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Assembly.GameSystem.Message;
using Utilities;

namespace Assembly.Components.StageGimmicks
{
  public class Transformer : MonoBehaviour, IMessageListener
  {
#if DEBUG_TRANSFORMER
    [Header("[Debug Inspector]\ndon't forget to turn symbol DEBUG_TRANSFORMER off.")]
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

    [SerializeField] GameObject _target;
    [SerializeField] EzLerp animateProgress;

#if DEBUG_TRANSFORMER
    [Header("Debug")]
#endif
#if DEBUG_TRANSFORMER
    [SerializeField]
#endif
    float timescalePower = 1;
#if DEBUG_TRANSFORMER
    [SerializeField]
#endif
    float timescaleSignal = 0;

    Vector3 _positionDefault;

    void UpdateTimeScale()
      => animateProgress.localTimeScale = inverseSignal
        ? timescalePower * (1 - timescaleSignal)
        : timescalePower * timescaleSignal;

    void Start()
    {
      _positionDefault = _target.transform.localPosition;

      UpdateTimeScale();

      this.FixedUpdateAsObservable()
        .Where(animateProgress.isNeedsCalc)
        .Subscribe(_ => UpdatePosition(_target.transform, animateProgress));

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
              UpdatePosition(_target.transform, message.intensity);
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
      UpdateTimeScale();
    }

    void UpdatePosition(Transform transform, MixFactor intensity)
    {
      transform.localPosition = intensity.UpdAdd(_positionDefault, positionDelta);
    }

#if UNITY_EDITOR
    MeshFilter _plateMeshFilter;
    Mesh _plateMesh;
    Vector3 _positionDefaultGlobal;
    void OnDrawGizmos()
    {
      Vector3 positionDelta = this.positionDelta;

      Gizmos.color = Color.white;
      if (!Application.isPlaying || _positionDefaultGlobal == Vector3.zero)
      { _positionDefaultGlobal = _target.transform.position; }
      Gizmos.DrawLine(_positionDefaultGlobal, _positionDefaultGlobal + positionDelta);
      if (_target)
      {
        _plateMeshFilter = _target.GetComponent<MeshFilter>();
        if (_plateMeshFilter)
        {
          if (!_plateMesh) { _plateMesh = _plateMeshFilter.sharedMesh; }
          if (_plateMesh)
          {
            Gizmos.DrawMesh(_plateMesh, _positionDefaultGlobal + positionDelta, _target.transform.rotation, _target.transform.localScale);
          }
          return;
        }
        Gizmos.DrawSphere(_positionDefaultGlobal + positionDelta, .1f);
      }
    }
#endif
  }
}

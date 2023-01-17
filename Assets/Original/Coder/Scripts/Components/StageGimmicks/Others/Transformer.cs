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
    enum OperationMode
    {
      FollowIntensity = 1 << 0,
      PingPong = 1 << 1,
      Invoke = 1 << 2,
    }
    [Header("General")]

    [SerializeField] OperationMode mode;
    [SerializeField] bool inverseSignal;
    [SerializeField] Vector3 _positionDelta;
    Vector3 positionDelta => transform.localRotation * _positionDelta;

    [SerializeField] GameObject _target;
    [SerializeField] EzLerp animateProgress;

    [Header("For Invoke Mode")]

    [SerializeField][Range(0, 0.99f)] float signalThrethold = .5f;

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
    float timescalePingPong = 1;

    Vector3 _positionDefault;

    void UpdateTimeScale()
      => animateProgress.localTimeScale = timescalePower * timescalePingPong;

    void Start()
    {
      _positionDefault = _target.transform.localPosition;

      switch (mode)
      {
        case OperationMode.Invoke:
        case OperationMode.FollowIntensity:
          if (inverseSignal)
          {
            animateProgress.SetFactor1();
            animateProgress.SetAsIncrease();
          }
          break;
        case OperationMode.PingPong:
          if (inverseSignal)
          { animateProgress.SetAsIncrease(); }
          else
          { timescalePingPong = 0; }
          break;
      }
      UpdateTimeScale();

      this.FixedUpdateAsObservable()
        .Where(animateProgress.isNeedsCalc)
        .Subscribe(_ =>
        {
          _target.transform.localPosition = animateProgress.UpdAdd(_positionDefault, positionDelta);
        });

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
              _target.transform.localPosition =
                _positionDefault + positionDelta * message.intensity.Invpeek(inverseSignal);
              break;
            case OperationMode.PingPong:
              timescalePingPong = message.intensity.Invpeek(inverseSignal);
              UpdateTimeScale();
              break;
            case OperationMode.Invoke:
              animateProgress.SetMode(
                (message.intensity.PeekFactor() >= signalThrethold) ^ inverseSignal);
              break;
          }
          break;
      }
    }
    public void Powered(MixFactor powerGain)
    {
      timescalePower = powerGain.PeekFactor();
      UpdateTimeScale();
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

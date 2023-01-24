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
    enum OnPowerNotEnough { StayHere, GoBack }
    enum OnSignalNotEnough { StayHere, GoBack }
    enum OperationMode
    {
      FollowIntensity = 1 << 0,
      PingPong = 1 << 1,
      Invoke = 1 << 2,
    }
    [Header("General")]

    [SerializeField] OperationMode mode;
    [SerializeField] OnPowerNotEnough onPowerNotEnough = OnPowerNotEnough.StayHere;
    [SerializeField] OnSignalNotEnough onSignalNotEnough = OnSignalNotEnough.GoBack;
    [SerializeField] bool inverseSignal;
    [Tooltip("inverseSignalかつprewarmの時、移動先からスタートする")]
    [SerializeField] bool prewarm;
    [SerializeField] Vector3 _positionDelta;
    Vector3 positionDelta => transform.localRotation * _positionDelta;

    [SerializeField] GameObject _target;
    [SerializeField] EzLerp animateProgress;

    [SerializeField][Range(0f, 0.99f)] float signalThrethold = 0f;
    [SerializeField][Range(0f, 0.99f)] float powerThrethold = .5f;

    bool hasEnoughSignal = false;
    bool hasEnoughPower = true;


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
    float timescaleSignal = 1;

    Vector3 _positionDefault;

    void Start()
    {
      _positionDefault = _target.transform.localPosition;

      switch (mode)
      {
        case OperationMode.FollowIntensity:
          if (inverseSignal)
          {
            if (prewarm) animateProgress.SetFactor1();
            animateProgress.SetAsIncrease();
          }
          break;
        case OperationMode.Invoke:
        case OperationMode.PingPong:
          hasEnoughSignal = inverseSignal;
          if (hasEnoughSignal)
          {
            if (prewarm) animateProgress.SetFactor1();
            animateProgress.SetAsIncrease();
          }
          else
          { timescaleSignal = 0; }
          UpdateAnimateProgress();

          break;
      }

      this.FixedUpdateAsObservable()
        .Subscribe(_ =>
        {
          if (animateProgress.needsCalc)
          {
            _target.transform.localPosition = animateProgress.UpdAdd(_positionDefault, positionDelta);
          }
          else
          {
            if (mode == OperationMode.PingPong)
            {
              if (hasEnoughPower && hasEnoughSignal)
                animateProgress.FlipMode();
            }
          }
        });
    }

    public void ReceiveSignal(MixFactor signal)
    {
      hasEnoughSignal = signal.PeekFactor() > signalThrethold ^ inverseSignal;
      timescaleSignal = signal.Invpeek(inverseSignal);

      switch (mode)
      {
        case OperationMode.FollowIntensity:
          _target.transform.localPosition =
            _positionDefault + positionDelta * signal.Invpeek(inverseSignal);
          return;
      }
      UpdateAnimateProgress();
    }
    public void Powered(MixFactor powerGain)
    {
      hasEnoughPower = powerGain.PeekFactor() > powerThrethold;
      timescalePower = powerGain.PeekFactor();
      UpdateAnimateProgress();
    }

    void UpdateAnimateProgress()
    {
      switch (onSignalNotEnough)
      {
        case OnSignalNotEnough.GoBack:
          switch (onPowerNotEnough)
          {
            case OnPowerNotEnough.StayHere:
              animateProgress.localTimeScale = timescalePower;
              animateProgress.SetMode(hasEnoughSignal);
              break;
            case OnPowerNotEnough.GoBack:
              animateProgress.SetMode(hasEnoughPower && hasEnoughSignal);
              break;
          }
          break;
        case OnSignalNotEnough.StayHere:
          switch (onPowerNotEnough)
          {
            case OnPowerNotEnough.StayHere:
              animateProgress.localTimeScale = timescalePower * timescaleSignal;
              break;
            case OnPowerNotEnough.GoBack:
              animateProgress.localTimeScale = timescaleSignal;
              animateProgress.SetMode(hasEnoughPower);
              break;
          }
          break;

      }
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

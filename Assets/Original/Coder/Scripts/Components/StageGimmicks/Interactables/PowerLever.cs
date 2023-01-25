using UnityEngine;
using Utilities;

namespace Assembly.Components.StageGimmicks
{
  [RequireComponent(typeof(SafetyTrigger))]
  public class PowerLever : InteractablePower
  {
    [SerializeField] Transform _leverRoot;
    [SerializeField] Quaternion _leverRotateFrom;
    [SerializeField] Quaternion _leverRotateTo;

    protected override void OnProgressUpdate(MixFactor factor)
    {
      _leverRoot.localRotation = factor.Mix(_leverRotateFrom, _leverRotateTo);
    }
  }
}
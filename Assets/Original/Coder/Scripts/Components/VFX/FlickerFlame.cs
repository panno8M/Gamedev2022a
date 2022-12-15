using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class FlickerFlame : MonoBehaviour
{
  Light _light;
  float _defaultIntensity;
  [SerializeField] float yShift;
  [SerializeField] float shiftSpeed = 1;
  [SerializeField] AnimationCurve curve = new AnimationCurve();

  void Start()
  {
    _light = GetComponent<Light>();
    _defaultIntensity = _light.intensity;
    Observable.EveryUpdate()
    .Subscribe(_ =>
    {
      _light.intensity = _defaultIntensity * curve.Evaluate(Mathf.PerlinNoise(shiftSpeed * Time.time, yShift));
    }).AddTo(this);
  }
}

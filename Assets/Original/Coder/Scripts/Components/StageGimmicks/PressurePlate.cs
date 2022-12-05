using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class PressurePlate : MonoBehaviour
{

  List<Collider> riddenCounter = new List<Collider>();

  enum Mode { Relax = -1, Press = 1 }
  Mode targetMode = Mode.Relax;
  float animateProgress = 0;

  [SerializeField] GameObject _plateObject;
  Material _plateMaterial;
  Vector3 _positionDefault;
  [SerializeField] Vector3 _positionDelta;
  [SerializeField] float _secDuration;
  Color _relaxColor;
  [SerializeField] Color _pressColor;


  void Start()
  {
    _positionDefault = _plateObject.transform.localPosition;
    _plateMaterial = _plateObject.GetComponent<Renderer>().material;
    _relaxColor = _plateMaterial.color;
    AnimatePress().Forget();
  }
  void OnTriggerEnter(Collider other)
  {
    riddenCounter.Add(other);
    targetMode = Mode.Press;
  }
  void OnTriggerExit(Collider other)
  {
    riddenCounter.RemoveAll(x => x == other);
    if (riddenCounter.Count == 0)
    {
      targetMode = Mode.Relax;
    }
  }

  async UniTask AnimatePress()
  {
    while (Application.isPlaying)
    {
      animateProgress = Mathf.Clamp01(animateProgress + (int)targetMode * Time.fixedDeltaTime / _secDuration);
      _plateObject.transform.localPosition = _positionDefault + _positionDelta * animateProgress;
      if (_plateMaterial)
      {
        _plateMaterial.color = Color.Lerp(_relaxColor, _pressColor, animateProgress);
      }
      await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
    }
    _plateObject.transform.localPosition = _positionDefault;
  }
}

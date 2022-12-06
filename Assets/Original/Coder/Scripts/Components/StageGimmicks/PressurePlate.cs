using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class PressurePlate : MonoBehaviour
{

  List<Collider> riddenCounter = new List<Collider>();

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

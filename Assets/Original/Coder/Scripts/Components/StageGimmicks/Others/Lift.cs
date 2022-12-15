using UnityEngine;
using Assembly.GameSystem.Message;

namespace Assembly.Components.StageGimmicks
{
  public class Lift : MonoBehaviour, IMessageReceiver
  {
    Vector3 _positionDefault;
    [SerializeField] Vector3 _positionDelta;
    [SerializeField][Range(0, 1)] float _power;

    [SerializeField] GameObject _plateObject;
    Material _plateMaterial;
    Color _relaxColor;
    [SerializeField] Color _acitivatedColor;
    void Start()
    {
      _positionDefault = transform.localPosition;
      _plateMaterial = _plateObject.GetComponent<Renderer>().material;
      _relaxColor = _plateMaterial.color;
    }

    public void ReceiveMessage(MessageUnit message)
    {
      switch (message.kind)
      {
        case MessageKind.Signal:
          transform.localPosition = message.intensity.UpdAdd(_positionDefault, _positionDelta * _power);
          break;
        case MessageKind.Power:
          _power = message.intensity.UpdFactor();
          _plateMaterial.color = message.intensity.Mix(_relaxColor, _acitivatedColor);
          break;
      }
    }
  }
}

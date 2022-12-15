using UnityEngine;
using Assembly.GameSystem.Message;

namespace Assembly.Components.StageGimmicks
{
  public class Lift : MonoBehaviour, IMessageReceiver
  {
    Vector3 _positionDefault;
    [SerializeField] Vector3 _positionDelta;
    void Start()
    {
      _positionDefault = transform.localPosition;
    }

    public void ReceiveMessage(MessageUnit message)
    {
      transform.localPosition = message.intensity.Add(_positionDefault, _positionDelta);
    }
  }
}

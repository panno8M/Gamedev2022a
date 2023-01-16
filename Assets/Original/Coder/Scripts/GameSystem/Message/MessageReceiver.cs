#if UNITY_EDITOR
// #define DEBUG_MESSAGE
#endif
using System;
using UnityEngine;
using UniRx;
using Utilities;

namespace Assembly.GameSystem.Message
{
  public class MessageReceiver : MonoBehaviour
  {
    enum PowerUsage { Add, Ignore }
    [SerializeField] PowerUsage powerUsage;
#if DEBUG_MESSAGE
    [SerializeField]
#endif
    float powerGainUnnormalized;
    MixFactor powerGain = new MixFactor();

    Subject<MessageUnit> _OnMessageRecieve = new Subject<MessageUnit>();
    IObservable<MessageUnit> OnMessageRecieve => _OnMessageRecieve;

    Subject<MixFactor> _OnPowered = new Subject<MixFactor>();

    void Start()
    {
      foreach (IMessageListener receiver in gameObject.GetComponentsInChildren<IMessageListener>())
      {
        OnMessageRecieve.Subscribe(receiver.ReceiveMessage);
        _OnPowered.Subscribe(receiver.Powered);
      }
      Supply(0);
    }

    public void Recieve(MessageUnit message)
    {
      _OnMessageRecieve.OnNext(message);
    }

    public void Supply(float deltaWatts)
    {
      switch (powerUsage)
      {
        case PowerUsage.Ignore:
          powerGainUnnormalized = 1;
          break;
        case PowerUsage.Add:
          powerGainUnnormalized += deltaWatts;
          break;
      }
      powerGain.SetFactor(powerGainUnnormalized);
      _OnPowered.OnNext(powerGain);
    }
  }
}

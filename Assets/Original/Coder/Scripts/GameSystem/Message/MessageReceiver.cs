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
    enum PowerUsage { Add, AlwaysOn }
    [SerializeField] PowerUsage powerUsage;
#if DEBUG_MESSAGE
    [SerializeField]
#endif
    float signalGainUnnormalized;
    float powerGainUnnormalized;

    MixFactor signalGain = new MixFactor();
    MixFactor powerGain = new MixFactor();

    Subject<MixFactor> _OnRecieveSignal = new Subject<MixFactor>();
    Subject<MixFactor> _OnPowered = new Subject<MixFactor>();

    void Start()
    {
      foreach (IMessageListener receiver in gameObject.GetComponentsInChildren<IMessageListener>())
      {
        _OnRecieveSignal.Subscribe(receiver.ReceiveSignal);
        _OnPowered.Subscribe(receiver.Powered);
      }
      Supply(0);
    }

    public void Recieve(float deltaSignal)
    {
      signalGainUnnormalized += deltaSignal;
      signalGain.SetFactor(signalGainUnnormalized);
      _OnRecieveSignal.OnNext(signalGain);
    }

    public void Supply(float deltaWatts)
    {
      switch (powerUsage)
      {
        case PowerUsage.AlwaysOn:
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

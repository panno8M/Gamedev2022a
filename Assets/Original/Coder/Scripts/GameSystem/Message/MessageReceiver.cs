using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Assembly.GameSystem.Message
{
  public class MessageReceiver : MonoBehaviour
  {
    Subject<MessageUnit> _OnMessageRecieve = new Subject<MessageUnit>();
    IObservable<MessageUnit> OnMessageRecieve => _OnMessageRecieve;

    void Start()
    {
      foreach (IMessageListener receiver in gameObject.GetComponentsInChildren<IMessageListener>())
      {
        OnMessageRecieve.Subscribe(receiver.ReceiveMessage);
      }
    }

    public void Recieve(MessageUnit message)
    {
      _OnMessageRecieve.OnNext(message);
    }
  }
}

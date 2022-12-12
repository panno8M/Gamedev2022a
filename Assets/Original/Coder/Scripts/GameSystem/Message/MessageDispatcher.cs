using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Utilities;

namespace Assembly.GameSystem.Message
{
  [Serializable]
  public class MessageDispatcher
  {
    public List<MessageReceiver> receivers = new List<MessageReceiver>();

    public MessageUnit message = new MessageUnit();

    public void Dispatch(MessageUnit message)
    {
      foreach (MessageReceiver receiver in receivers)
      {
        receiver.Recieve(message);
      }
    }
    public void Dispatch() { Dispatch(message); }



    public void DrawArrow(Transform transform)
    {
      Gizmos.color = Color.green;
      foreach (MessageReceiver receiver in receivers)
      {
        GizmosEx.DrawArrow(transform.position, receiver.transform.position);
      }
    }

  }
}

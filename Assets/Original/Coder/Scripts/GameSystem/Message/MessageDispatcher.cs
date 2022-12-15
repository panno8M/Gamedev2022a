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

    public MessageDispatcher(MessageKind kind)
    {
      message.kind = kind;
    }
    public MessageDispatcher()
    {
      message.kind = MessageKind.Signal;
    }

    public void Dispatch(MessageUnit message)
    {
      foreach (MessageReceiver receiver in receivers)
      {
        receiver.Recieve(message);
      }
    }
    public void Dispatch() { Dispatch(message); }



    public void DrawArrow(Transform transform, MessageUnit message)
    {
      Gizmos.color =
        message.kind == MessageKind.Signal ? Color.green :
        message.kind == MessageKind.Power ? Color.red :
        Color.gray;

      foreach (MessageReceiver receiver in receivers)
      {
        if (!receiver) { continue; }
        GizmosEx.DrawArrow(transform.position, receiver.transform.position);
      }
    }
    public void DrawArrow(Transform transform)
    {
      DrawArrow(transform, message);
    }

  }
}

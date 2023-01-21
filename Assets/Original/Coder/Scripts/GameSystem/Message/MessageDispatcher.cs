using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Assembly.GameSystem.Message
{
  [Serializable]
  public class MessageDispatcher
  {
    float lastTime;
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
      lastTime = Time.time;
      foreach (MessageReceiver receiver in receivers)
      {
        receiver.Recieve(message);
      }
    }
    public void Dispatch() { Dispatch(message); }


    public void DrawArrow(Transform transform, string label)
    {
      Gizmos.color = (Time.time - lastTime < 0.1) ? Color.yellow : Color.green;

      foreach (MessageReceiver receiver in receivers)
      {
        if (!receiver) { continue; }
        GizmosEx.DrawArrow(transform.position, receiver.transform.position);
        UnityEditor.Handles.Label((transform.position + receiver.transform.position) / 2, label);
      }
    }
  }

  [Serializable]
  public class PowerSupplier
  {
    float watts;
    public List<MessageReceiver> receivers = new List<MessageReceiver>();
    public void Supply(MixFactor watts)
    {
      float currentWatts = watts.PeekFactor();
      float deltaWatts = currentWatts - this.watts;
      foreach (MessageReceiver receiver in receivers)
      { receiver.Supply(deltaWatts); }
      this.watts = currentWatts;
    }

    public void DrawArrow(Transform transform, string label)
    {
      Gizmos.color = Color.red;

      foreach (MessageReceiver receiver in receivers)
      {
        if (!receiver) { continue; }
        GizmosEx.DrawArrow(transform.position, receiver.transform.position);
        UnityEditor.Handles.Label((transform.position + receiver.transform.position) / 2, label);
      }
    }
  }
}

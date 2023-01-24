using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Assembly.GameSystem.Message
{
  [Serializable]
  public class MessageDispatcher
  {
#if UNITY_EDITOR
    float lastTime;
#endif
    float signal;

    public List<MessageReceiver> receivers = new List<MessageReceiver>();
    // public MessageUnit message = new MessageUnit();

    public void Dispatch(MixFactor signal)
    {
#if UNITY_EDITOR
      lastTime = Time.time;
#endif
      float cur = signal.PeekFactor();
      float delta = cur - this.signal;
      foreach (MessageReceiver receiver in receivers)
      {
        receiver.Recieve(delta);
      }
      this.signal = cur;
    }


#if UNITY_EDITOR
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
#endif
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
      {
        if (!receiver)
        {
          Debug.LogWarning($"found missing power receiver");
          continue;
        }
        receiver.Supply(deltaWatts);
      }
      this.watts = currentWatts;
    }

#if UNITY_EDITOR
    public void DrawArrow(Transform transform, string label)
    {
      Gizmos.color = Color.red;

      foreach (MessageReceiver receiver in receivers)
      {
        if (!receiver) { continue; }
        GizmosEx.DrawArrow(transform.position, receiver.transform.position);
        // UnityEditor.Handles.Label((transform.position + receiver.transform.position) / 2, label);
      }
    }
#endif
  }
}

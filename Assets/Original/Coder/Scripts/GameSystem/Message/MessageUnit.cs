#if UNITY_EDITOR
// #define DEBUG_MESSAGE_UNIT
#endif

using System;
using UnityEngine;
using Utilities;

namespace Assembly.GameSystem.Message
{
  [Serializable]
  public class MessageUnit
  {
    public MessageKind kind;
    #if !DEBUG_MESSAGE_UNIT
    [HideInInspector]
    #endif
    public MixFactor intensity = new MixFactor();
  }
  public enum MessageKind
  {
    Signal,
    Power,
    Invoke,
  }
}

using System;
using UnityEngine;
using Utilities;

namespace Assembly.GameSystem.Message
{
  public class MessageUnit
  {
    public MessageKind kind;
    public MixFactor intensity = new MixFactor();
  }
  public enum MessageKind
  {
    Signal,
  }
}

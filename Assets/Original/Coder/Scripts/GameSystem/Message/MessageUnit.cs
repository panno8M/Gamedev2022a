using System;
using Utilities;

namespace Assembly.GameSystem.Message
{
  [Serializable]
  public class MessageUnit
  {
    public MessageKind kind;
    public MixFactor intensity = new MixFactor();
  }
  public enum MessageKind
  {
    Signal,
    Power,
  }
}

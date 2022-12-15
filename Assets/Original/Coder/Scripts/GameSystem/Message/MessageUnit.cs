using Utilities;

namespace Assembly.GameSystem.Message
{
  public class MessageUnit
  {
    public MessageKind kind;
    public MixFactor intensity;
  }
  public enum MessageKind
  {
    Signal,
  }
}

namespace Assembly.GameSystem.Message
{
  public class MessageUnit
  {
    public MessageKind kind = MessageKind.None;
    public float signalPower = 1;
  }
  [System.Flags]
  public enum MessageKind
  {
    None = 0,
    Everything = 0b0,
  }
}

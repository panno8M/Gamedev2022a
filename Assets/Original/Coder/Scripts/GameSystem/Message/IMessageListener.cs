using Utilities;

namespace Assembly.GameSystem.Message
{
  public interface IMessageListener
  {
    public void ReceiveMessage(MessageUnit message);
    public void Powered(MixFactor powerGain);
  }
}
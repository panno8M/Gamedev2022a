using Utilities;

namespace Assembly.GameSystem.Message
{
  public interface IMessageListener
  {
    public void ReceiveSignal(MixFactor message);
    public void Powered(MixFactor powerGain);
  }
}
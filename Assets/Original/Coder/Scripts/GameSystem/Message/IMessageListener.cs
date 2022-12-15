using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assembly.GameSystem.Message
{
  public interface IMessageListener
  {
    public void ReceiveMessage(MessageUnit message);
  }
}
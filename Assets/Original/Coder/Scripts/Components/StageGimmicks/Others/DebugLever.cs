using UnityEngine;
using Assembly.GameSystem.Message;

public class DebugLever : MonoBehaviour, IMessageListener
{
    public void ReceiveMessage(MessageUnit message)
    {   
        Vector3 t = gameObject.transform.localScale;
        t.x = message.intensity.UpdMix(1, -1);
        gameObject.transform.localScale = t;
    }
}

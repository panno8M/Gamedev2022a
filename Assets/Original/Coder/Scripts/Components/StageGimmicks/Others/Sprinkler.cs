using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assembly.GameSystem.Message;

public class Sprinkler : MonoBehaviour, IMessageReceiver
{
  [SerializeField] ParticleSystem _psWater;
  ParticleSystem.EmissionModule emission;
  float defaultRate;

  void Start()
  {
    emission = _psWater.emission;
    defaultRate = emission.rateOverTime.constant;
  }

  public void ReceiveMessage(MessageUnit message)
  {
    emission.rateOverTime = message.intensity.Mix(defaultRate, 0);
  }
}

using UnityEngine;
using Assembly.GameSystem.Message;
using Utilities;

public class Sprinkler : MonoBehaviour, IMessageListener
{
  [SerializeField] ParticleSystem _psWater;
  ParticleSystem.EmissionModule emission;
  float defaultRate;

  void Start()
  {
    emission = _psWater.emission;
    defaultRate = emission.rateOverTime.constant;
  }

  public void ReceiveSignal(MixFactor signal)
  {
    emission.rateOverTime = signal.Mix(defaultRate, 0);
  }
  public void Powered(MixFactor powerGain) { }
}

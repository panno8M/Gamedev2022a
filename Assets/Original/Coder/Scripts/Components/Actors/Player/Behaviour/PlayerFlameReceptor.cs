using UnityEngine;

namespace Assembly.Components.Actors.Player
{
  public class PlayerFlameReceptor : ActorBehavior<PlayerAct>
  {
    [SerializeField] ParticleSystem _flame;
    [SerializeField] FlickerFlame _flicker;
    ParticleSystem.EmissionModule emission;
    float defaultRate;

    [SerializeField][Range(0, 1)] float _flameQuantity;

    public float flameQuantity
    {
      get { return _flameQuantity; }
      set
      {
        _flameQuantity = value;
        emission.rateOverTime = defaultRate * value;
        _flicker.weight = value;
      }
    }

    protected override void Blueprint()
    {
      emission = _flame.emission;
      defaultRate = emission.rateOverTime.constant;
    }
  }
}
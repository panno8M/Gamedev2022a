using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace Assembly.Components.Actors.Behaviour
{
  public class PlayerBreath : MonoBehaviour
  {
    [SerializeField] ParticleSystem psFlameBreath;

    void Awake()
    {
      var player = Global.Player;
      this.FixedUpdateAsObservable()
          .Where(_ => Global.Control.BreathInput.Value)
          .Subscribe(_ =>
          {
            psFlameBreath.transform.LookAt(Global.Control.MousePosStage.Value);
          }).AddTo(this);

      player.OnBreathStart
          .Subscribe(_ => psFlameBreath.Play()).AddTo(this);
      player.OnBreathStop
          .Subscribe(_ => psFlameBreath.Stop()).AddTo(this);
    }
  }
}
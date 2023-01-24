using UnityEngine;
using Cysharp.Threading.Tasks;
using Assembly.GameSystem;

namespace Assembly.Components
{
  [RequireComponent(typeof(Collider))]
  public class StageTransitionVolume : MonoBehaviour
  {
    [Zenject.Inject]
    public void Construct(
      StageTransitionMaster master,
      UI.SimpleFader fader)
    {
      this.master = master;
      this.fader = fader;
    }
    StageTransitionMaster master;
    UI.SimpleFader fader;


    void OnTriggerEnter(Collider other)
    {
      if (!other.CompareTag(Tag.Player.GetName())) { return; }
      Transition().Forget();
    }

    async UniTask Transition()
    {
      await fader.Fade(1);
      master.Transition();
    }
  }
}
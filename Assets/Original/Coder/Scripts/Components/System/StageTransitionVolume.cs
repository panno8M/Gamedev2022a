using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using Assembly.GameSystem;
using Assembly.Components.StageGimmicks;

namespace Assembly.Components
{
  [RequireComponent(typeof(Collider))]
  [RequireComponent(typeof(SafetyTrigger))]
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
    SafetyTrigger _trigger;

    Kandelaar __kandelaar;

    bool done;

    void Awake()
    {
      _trigger = GetComponent<SafetyTrigger>();
    }

    void Start()
    {
      _trigger.OnEnter.Subscribe(other =>
      {
        if (!other.CompareTag(Tag.Kandelaar.GetName())) { return; }
        __kandelaar = other.GetComponent<Kandelaar>();
      });

      _trigger.OnExit.Subscribe(other =>
      {
        if (!__kandelaar || other.gameObject == __kandelaar.gameObject) { return; }
        __kandelaar = null;
      });

    }

    void Update()
    {
      if (__kandelaar && __kandelaar.holdable.owner && !done)
      {
        Transition().Forget();
        done = true;
      }
    }

    async UniTask Transition()
    {
      await fader.Fade(1);
      master.Transition();
    }
  }
}
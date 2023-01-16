using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Assembly.GameSystem.Input;
using Assembly.Components.Actors.Player;
using Utilities;

namespace Assembly.Components.UI
{
  public class PlayerCanvas : MonoBehaviour
  {
    InputControl control;
    PlayerAct player;
    [Zenject.Inject]
    public void DepsInject(InputControl control, PlayerAct player)
    {
      this.control = control;
      this.player = player;
    }

    [SerializeField] GameObject uiQuestion;

    void Start()
    {
      player.OnEnableAsObservable()
          .Subscribe(_ => gameObject.SetActive(true)).AddTo(this);
      player.OnDisableAsObservable()
          .Subscribe(_ => gameObject.SetActive(false)).AddTo(this);

      Observable
          .EveryUpdate()
          .Select(_ => !player.hand.holder.hasItem && player.hand.holder.accessibles.Count != 0)
          .DistinctUntilChanged()
          .Subscribe(b => uiQuestion.SetActive(b));
    }
    void LateUpdate()
    {
      transform.position = player.transform.position;
    }
  }
}
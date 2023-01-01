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

    [SerializeField] GameObject uiRightArrow;
    [SerializeField] GameObject uiLeftArrow;
    [SerializeField] GameObject uiQuestion;

    Vector3 posR;
    Vector3 posL;
    [SerializeField] float moveDelta;
    [SerializeField] EzLerp easeR = new EzLerp(1);
    [SerializeField] EzLerp easeL = new EzLerp(1);

    void Start()
    {
      posR = uiRightArrow.transform.localPosition;
      posL = uiLeftArrow.transform.localPosition;

      player.OnEnableAsObservable()
          .Subscribe(_ => gameObject.SetActive(true)).AddTo(this);
      player.OnDisableAsObservable()
          .Subscribe(_ => gameObject.SetActive(false)).AddTo(this);

      Observable
          .EveryUpdate()
          .Select(_ => !player.hand.holder.hasItem && player.hand.holder.accessibles.Count != 0)
          .DistinctUntilChanged()
          .Subscribe(b => uiQuestion.SetActive(b));


      Observable.EveryUpdate()
        .Select(_ => control.HorizontalMoveInput.Value)
        .Subscribe(hmi =>
        {
          easeR.SetMode(increase: hmi == 1);
          easeL.SetMode(increase: hmi == -1);

          if (!player.gameObject.activeSelf) { return; }

          uiRightArrow.transform.localPosition = easeR.UpdAddX(posR, moveDelta);
          uiLeftArrow.transform.localPosition = easeL.UpdAddX(posL, -moveDelta);
        }).AddTo(this);
    }
    void LateUpdate()
    {
      transform.position = player.transform.position;
    }
  }
}
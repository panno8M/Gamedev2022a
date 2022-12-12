using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Assembly.Components.Actors;
using Utilities;

namespace Assembly.Components.UI
{
  public class PlayerCanvas : MonoBehaviour
  {
    PlayerAct player;
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
      player = Global.Player;

      posR = uiRightArrow.transform.localPosition;
      posL = uiLeftArrow.transform.localPosition;

      player.OnEnableAsObservable()
          .Subscribe(_ => gameObject.SetActive(true)).AddTo(this);
      player.OnDisableAsObservable()
          .Subscribe(_ => gameObject.SetActive(false)).AddTo(this);

      Observable
          .EveryUpdate()
          .Select(_ => !player.holder.hasItem && player.holder.accessibles.Count != 0)
          .DistinctUntilChanged()
          .Subscribe(b => uiQuestion.SetActive(b));


      Observable.EveryUpdate()
        .Select(_ => Global.Control.HorizontalMoveInput.Value)
        .Subscribe(hmi =>
        {
          easeR.SetAsIncrease(hmi == 1);
          easeL.SetAsIncrease(hmi == -1);

          if (!player.gameObject.activeSelf) { return; }

          uiRightArrow.transform.localPosition = easeR.AddX(posR, moveDelta);
          uiLeftArrow.transform.localPosition = easeL.AddX(posL, -moveDelta);
        }).AddTo(this);
    }
    void LateUpdate()
    {
      transform.position = player.transform.position;
    }
  }
}
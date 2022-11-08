using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Assembly.Components.Actors;

namespace Assembly.Components.UI
{
  public class TutorialMgr : MonoBehaviour
  {
    Player player;
    Transform playerTrans;
    [SerializeField] GameObject uiPlayerPivot;
    [SerializeField] GameObject uiRightArrow;
    [SerializeField] GameObject uiLeftArrow;
    [SerializeField] GameObject uiQuestion;

    Vector3 posR;
    Vector3 posL;
    [SerializeField] float moveDelta;

    float easeR;
    float easeL;
    // Start is called before the first frame update
    void Start()
    {
      player = Global.Player;
      playerTrans = player.transform;

      posR = uiRightArrow.transform.localPosition;
      posL = uiLeftArrow.transform.localPosition;

      player.OnEnableAsObservable()
          .Subscribe(_ => uiPlayerPivot.SetActive(true)).AddTo(this);
      player.OnDisableAsObservable()
          .Subscribe(_ => uiPlayerPivot.SetActive(false)).AddTo(this);

      Observable.EveryUpdate()
        .Select(_ => Global.Control.HorizontalMoveInput.Value)
        .Subscribe(hmi =>
        {
          easeR = Mathf.Lerp(easeR, (hmi == -1 ? 0 : hmi), 0.1f);
          easeL = Mathf.Lerp(easeL, (hmi == 1 ? 0 : hmi), 0.1f);

          if (!player.gameObject.activeSelf) { return; }

          uiRightArrow.transform.localPosition = new Vector3(posR.x + moveDelta * easeR, posR.y, posR.z);
          uiLeftArrow.transform.localPosition = new Vector3(posL.x + moveDelta * easeL, posL.y, posL.z);
        }).AddTo(this);

    }
    void LateUpdate()
    {
      uiPlayerPivot.transform.position = Camera.main.WorldToScreenPoint(playerTrans.position);
    }
  }
}

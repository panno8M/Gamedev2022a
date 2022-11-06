using UnityEngine;
using Cinemachine;
using UniRx;

namespace Assembly.Components.Utils
{
  [RequireComponent(typeof(CinemachineVirtualCamera))]
  public class FollowPointMover : MonoBehaviour
  {
    [SerializeField] Transform camFollowOnIdle;
    [SerializeField] Transform camFollowOnWalk;
    CinemachineVirtualCamera vcam;
    void Start()
    {
      vcam = GetComponent<CinemachineVirtualCamera>();

      Global.Control.HorizontalMoveInput
          .Subscribe(hmi => vcam.Follow.position = hmi == 0
                  ? camFollowOnIdle.position
                  : camFollowOnWalk.position)
          .AddTo(this);
    }
  }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UniRx;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class FollowPointMover : MonoBehaviour
{
    [SerializeField] Transform camFollowOnIdle;
    [SerializeField] Transform camFollowOnWalk;
    CinemachineVirtualCamera vcam;
    void Start() {
        vcam = GetComponent<CinemachineVirtualCamera>();
        // set camera position
        Player.Control.HorizontalMoveInput
            .Subscribe(hmi => vcam.Follow.position = hmi == 0
                       ? camFollowOnIdle.position
                       : camFollowOnWalk.position)
            .AddTo(this);
    }
}

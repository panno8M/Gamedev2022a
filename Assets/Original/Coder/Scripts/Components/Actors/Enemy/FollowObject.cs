using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
  public Transform target;
  [SerializeField] float moveSpeed = 0.03f;
  [SerializeField] Rigidbody rb;
  [SerializeField] Transform hose;
  void FixedUpdate()
  {
    if (!this || !isActiveAndEnabled) { return; }
    var rot = Quaternion.LookRotation(target.position - transform.position);
    if (transform.rotation != rot)
    {
      transform.rotation = Quaternion.RotateTowards(
        transform.rotation,
        rot,
        2f);
    }
    hose.LookAt(target);

    rb.MovePosition(transform.position + transform.forward * moveSpeed);
  }
}

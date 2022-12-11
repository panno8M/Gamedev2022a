using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assembly.Components.Actors
{
  public class FollowObject : MonoBehaviour
  {
    public Transform target;
    [SerializeField] float moveSpeed = 0.03f;
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform hose;
    [SerializeField] WaterEmitter _emitter;

    [SerializeField] float _closestDistance;
    [SerializeField] float _furthestDistance;

    float sqrClosestDistance => _closestDistance * _closestDistance;
    float sqrFurthestDistance => _furthestDistance * _furthestDistance;
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


      float sqrDistance = (target.position - transform.position).sqrMagnitude;
      if (sqrDistance < sqrClosestDistance)
      {
        MoveBackWard();
      }
      else if (sqrFurthestDistance < sqrDistance)
      {
        MoveForward();
      }
      else
      {
        _emitter.Launch();
      }
    }
    void MoveForward()
    {
      rb.MovePosition(transform.position + transform.forward * moveSpeed);
    }
    void MoveBackWard()
    {
      rb.MovePosition(transform.position - transform.forward * moveSpeed);
    }
  }
}
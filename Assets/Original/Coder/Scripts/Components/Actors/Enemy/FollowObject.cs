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
      var rot = Quaternion.LookRotation(target.position - transform.position);
      transform.rotation = Quaternion.RotateTowards(
        transform.rotation,
        rot,
        2f);
      hose.LookAt(target);


      float sqrDistance = (target.position - transform.position).sqrMagnitude;
      if (sqrDistance < sqrClosestDistance)
      {
        weight = -1;
      }
      else if (sqrFurthestDistance < sqrDistance)
      {
        weight = 1;
      }
      else { weight = 0; }

      Move();
      _emitter.Launch();
    }
    float weight;
    void Move()
    {
      transform.position += transform.forward * moveSpeed * weight * Time.deltaTime;
    }
  }
}
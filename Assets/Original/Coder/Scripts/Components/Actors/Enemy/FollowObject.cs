using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assembly.GameSystem;

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

    [SerializeField] float _relativeHeightFromGround;

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

      Vector3 dir;

      float sqrDistance = (target.position - transform.position).sqrMagnitude;
      if (sqrDistance < sqrClosestDistance)
      {
        dir = -transform.forward;
      }
      else if (sqrFurthestDistance < sqrDistance)
      {
        dir = transform.forward;
      }
      else
      {
        dir = Vector3.zero;
      }

      if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, _relativeHeightFromGround, new Layers(Layer.Stage)))
      {
        dir += Vector3.up;
      }

      Move(dir);
      _emitter.Launch();
    }
    void Move(Vector3 UnnormalizedDirection)
    {
      transform.position += UnnormalizedDirection.normalized * moveSpeed * Time.deltaTime;
    }
  }
}
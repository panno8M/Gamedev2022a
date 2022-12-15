using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Senses.Sight;
using UniRx;

namespace Assembly.Components.Actors
{
  public class AimModule : ActorBehavior<HostileDrone>
  {
    Transform _target;
    public Transform target => _target;

    public Transform hoseTransform;
    public Transform sightTransform;

    Quaternion defaultHoseRotation;
    Quaternion defaultSightRotation;

    public void Target(Transform newTarget)
    {
      _target = newTarget;
      if (!newTarget)
      {
        hoseTransform.localRotation = defaultHoseRotation;
        sightTransform.localRotation = defaultSightRotation;
      }
    }

    protected override void Blueprint()
    {
      defaultHoseRotation = hoseTransform.localRotation;
      defaultSightRotation = sightTransform.localRotation;

    }

    void FixedUpdate()
    {
      if (target)
      {
        hoseTransform.LookAt(target);
        sightTransform.LookAt(target);
      }
    }
  }
}

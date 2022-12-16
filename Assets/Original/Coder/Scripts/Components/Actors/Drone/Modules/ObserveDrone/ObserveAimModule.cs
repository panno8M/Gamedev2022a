using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Senses.Sight;
using UniRx;

namespace Assembly.Components.Actors
{
  public class ObserveAimModule : ActorBehavior<ObserveDrone>
  {
    Transform _target;
    public Transform target => _target;

    public Transform sightTransform;

    Quaternion defaultHoseRotation;
    Quaternion defaultSightRotation;

    public void Target(Transform newTarget)
    {
      _target = newTarget;
      if (!newTarget)
      {
        sightTransform.localRotation = defaultSightRotation;
      }
    }

    protected override void Blueprint()
    {
      defaultSightRotation = sightTransform.localRotation;

    }

    void FixedUpdate()
    {
      if (target)
      {
        sightTransform.LookAt(target);
      }
    }
  }
}

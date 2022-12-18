using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Assembly.GameSystem;

namespace Senses.Sight
{
  public class AiSight : MonoBehaviour
  {
    public float eyesight = 20;
    public float secSightCooldown = .1f;
    public float secToNotice = 1f;

    public Layer layerTarget = Layer.AiVisible;
    public Layer layerObstacle = Layer.Stage;
    public Layers lsEyeRay => new Layers(layerTarget, layerObstacle);
    public List<Tag> tags = new List<Tag>();

    ReactiveProperty<AiVisible> _InTheUnconscious = new ReactiveProperty<AiVisible>();
    ReactiveProperty<AiVisible> _InSight = new ReactiveProperty<AiVisible>();

    public AiVisible inTheUnconscious => _InTheUnconscious.Value;
    public AiVisible inSight => _InSight.Value;
    public IObservable<AiVisible> InSight => _InSight;

    float secElapsedViewing;

    void Start()
    {
      this.FixedUpdateAsObservable()
          .ThrottleFirst(TimeSpan.FromSeconds(secSightCooldown))
          .Subscribe(_ => LookFor());
    }

    bool Find(RaycastHit hit)
    {
      return hit.collider &&
          (hit.collider.gameObject.layer == (int)layerTarget) &&
          tags.Contains(hit.collider.gameObject.tag);
    }
    void LookFor()
    {
      var ray = new Ray(transform.position, transform.forward);
      RaycastHit hit;
      bool hitAny = Physics.Raycast(ray, out hit, eyesight, lsEyeRay);

      AiVisible previousSeen = inTheUnconscious;

      _InTheUnconscious.Value = (hitAny && Find(hit))
        ? hit.collider.GetComponent<AiVisible>()
        : null;

      if (inTheUnconscious && previousSeen == inTheUnconscious)
      {
        secElapsedViewing += secSightCooldown;
        if (secElapsedViewing > secToNotice)
        {
          _InSight.Value = inTheUnconscious;
          inSight.Find();
        }
      }
      else
      {
        if (inSight)
        {
          inSight.Find(false);
          _InSight.Value = null;
        }
        secElapsedViewing = 0;
      }


      Debug.DrawLine(
        ray.origin,
        (hitAny ? hit.point : ray.origin + ray.direction * eyesight),
        (inSight ? Color.red :
         inTheUnconscious ? Color.blue :
         Color.gray),
        secSightCooldown);


    }

    void OnDrawGizmos()
    {
      Gizmos.DrawRay(transform.position, transform.forward);
    }
  }
}
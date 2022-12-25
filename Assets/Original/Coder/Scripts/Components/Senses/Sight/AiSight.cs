using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Assembly.GameSystem;
using Utilities;

namespace Senses.Sight
{
  [RequireComponent(typeof(SphereCollider))]
  [RequireComponent(typeof(SafetyTrigger))]
  public class AiSight : MonoBehaviour
  {
    public float sightAngle = 30;
    public Layer layerObstacle = Layer.Stage;

    [SerializeField] ReactiveProperty<AiVisible> _InSight = new ReactiveProperty<AiVisible>();

    SafetyTrigger trigger;

    public AiVisible inSight
    {
      get { return _InSight.Value; }
      private set { _InSight.Value = value; }
    }

    public IObservable<AiVisible> InSight => _InSight;

    void Start()
    {
      trigger = GetComponent<SafetyTrigger>();

      this.OnEnableAsObservable()
        .Subscribe(_ =>
        {
          trigger.enabled = true;
        });
      this.OnDisableAsObservable()
        .Subscribe(_ =>
        {
          trigger.enabled = false;
          inSight = null;
        });

      trigger.OnStay
        .Where(_ => inSight)
        .Subscribe(_ =>
        {
          bool hitObstacle = HaveObstaclesInBetween(inSight.center);
          if (hitObstacle || !IsInAngle(inSight.center, sightAngle))
          { LostInSight(); }
        });

      trigger.OnStay
        .Where(_ => !inSight)
        .Subscribe(target =>
        {
          AiVisible visible = target.GetComponent<AiVisible>();
          if (!IsInAngle(visible.center, sightAngle)) { return; }

          bool hitObstacle = HaveObstaclesInBetween(visible.center);

          Debug.DrawLine(
            transform.position,
            visible.center.position,
            (hitObstacle ? Color.gray : Color.red), 1);

          if (!hitObstacle) { FoundOut(visible); }
        });
      trigger.OnExit
        .Where(_ => inSight)
        .Where(target => target.GetComponent<AiVisible>() == inSight)
        .Subscribe(_ => LostInSight());
    }

    bool HaveObstaclesInBetween(Transform target)
    {
      return Physics.Linecast(transform.position, target.position, new Layers(layerObstacle));
    }
    void FoundOut(AiVisible visible)
    {
      inSight = visible;
      inSight.Find();
    }
    void LostInSight()
    {
      inSight.Find(false);
      inSight = null;
    }

    bool IsInAngle(Transform target, float angle)
    {
      return (Vector3.Angle(transform.forward, target.position - transform.position) < angle);
    }

    void OnDrawGizmos()
    {
      Gizmos.DrawRay(transform.position, transform.forward);
      GizmosEx.DrawWireCircle(transform.position + transform.forward, transform.rotation, Mathf.Tan(sightAngle * Mathf.Deg2Rad));
    }
  }
}
#if UNITY_EDITOR
// #define DEBUG_SIGHT
#endif

using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Assembly.GameSystem;
using Assembly.Params;
using Utilities;

namespace Senses.Sight
{
  [RequireComponent(typeof(SphereCollider))]
  [RequireComponent(typeof(SafetyTrigger))]
  public class AiSight : DiBehavior
  {
    public SightParam param;
    [SerializeField]
    public Transform root;

#if DEBUG_SIGHT
    [SerializeField]
#endif
    ReactiveProperty<AiVisible> _InSight = new ReactiveProperty<AiVisible>();
#if DEBUG_SIGHT
    [SerializeField]
#endif
    ReactiveProperty<AiVisible> _Noticed = new ReactiveProperty<AiVisible>();

#if DEBUG_SIGHT
    [SerializeField]
#endif
    List<AiVisible> candidates = new List<AiVisible>();

    SafetyTrigger _trigger;
    int frameCount;

    public AiVisible inSight
    {
      get => _InSight.Value;
      private set => _InSight.Value = value;
    }
    public AiVisible noticed
    {
      get => _Noticed.Value;
      private set => _Noticed.Value = value;
    }

    public IObservable<AiVisible> InSight => _InSight;
    public IObservable<AiVisible> Noticed => _Noticed;

    EzLerp noticeProgress = new EzLerp();

    void Awake()
    {
      _trigger = GetComponent<SafetyTrigger>();
      noticeProgress.secDuration = param.secondsToNotice;
    }

    void OnEnable()
    {
      _trigger.enabled = true;
    }
    void OnDisable()
    {
      inSight = null;
      noticed = null;
      noticeProgress.SetFactor0();
      candidates.Clear();

      _trigger.enabled = false;
    }

    void Start()
    {
      this.FixedUpdateAsObservable()
        .Where(_ => isActiveAndEnabled)
        .Where(_ => !param.noticeImmediately)
        .Where(noticeProgress.isNeedsCalc)
        .Select(noticeProgress.UpdFactor)
        .Subscribe(fac =>
        {
          if (fac == 1 && !noticed) { noticed = inSight; }
          if (fac == 0 && noticed) { noticed = null; }
        });

      _trigger.OnStay
        .Subscribe(_ =>
        {
          if (frameCount++ < param.frameSkips) { return; }
          else { frameCount = 0; }

          if (inSight)
          {
            bool hitObstacle = HaveObstaclesInBetween(inSight.center);
            if (hitObstacle || !IsInAngle(inSight.center, param.angle))
            {
#if UNITY_EDITOR
              Debug.DrawLine(
                transform.position,
                inSight.center.position,
                (hitObstacle ? Color.green : Color.blue), 1);
#endif
              LostInSight();
            }
          }
          else
          {
            for (int i = 0; i < candidates.Count; i++)
            {
              AiVisible candidate = candidates[i];

              if (!IsInAngle(candidate.center, param.angle)) { continue; }

              bool hitObstacle = HaveObstaclesInBetween(candidate.center);

#if UNITY_EDITOR
              Debug.DrawLine(
                transform.position,
                candidate.center.position,
                (hitObstacle ? Color.gray : Color.red), 1);
#endif

              if (!hitObstacle)
              {
                FoundOut(candidate);
                return;
              }
            }
          }

        });

      _trigger.OnEnter
        .Subscribe(other =>
        {
          AiVisible visible = other.GetComponent<AiVisible>();
          if (!visible) { return; }
          candidates.Add(visible);
        });
      _trigger.OnExit
        .Where(_ => !param.allowWatchingOnExitSightArea)
        .Subscribe(target =>
        {
          if (inSight && target.gameObject == inSight.gameObject)
          {
            LostInSight();
          }
          candidates.RemoveAll(x => x.gameObject == target.gameObject);
        });
    }

    bool HaveObstaclesInBetween(Transform target)
    {
      return Physics.Linecast(transform.position, target.position, new Layers(param.obstacleLayer));
    }
    void FoundOut(AiVisible visible)
    {
      inSight = visible;
      inSight.Find();

      if (param.noticeImmediately)
      { noticed = visible; }
      else
      { noticeProgress.SetAsIncrease(); }
    }
    void LostInSight()
    {
      inSight.Find(false);
      inSight = null;

      if (param.noticeImmediately)
      { noticed = null; }
      else
      { noticeProgress.SetAsDecrease(); }
    }

    bool IsInAngle(Transform target, float angle)
    {
      return (Vector3.Angle(transform.forward, target.position - transform.position) < angle);
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
      Gizmos.DrawRay(transform.position, transform.forward);
      GizmosEx.DrawWireCircle(transform.position + transform.forward, transform.rotation, Mathf.Tan(param.angle * Mathf.Deg2Rad));
      GizmosEx.DrawWireCircle(transform.position + transform.forward / 2, transform.rotation, Mathf.Tan(param.angle * Mathf.Deg2Rad) / 2);
    }
#endif
  }
}
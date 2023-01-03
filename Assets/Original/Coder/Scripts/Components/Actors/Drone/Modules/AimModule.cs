using System;
using UnityEngine;
using UniRx;
using Senses.Sight;
using Assembly.GameSystem;
using Utilities;

namespace Assembly.Components.Actors
{
  public class AimModule : DiBehavior
  {
    [Serializable]
    struct SwipeSettings
    {
      public enum Mode { ToA, ToB }
      public Mode mode;
      public float angleA;
      public float angleB;
      public float speedFactor;

      public float angle => mode == Mode.ToA ? angleA : angleB;
      public float speed => speedFactor * Time.deltaTime;
      public Quaternion targetQuat(Transform standard) => Quaternion.AngleAxis(angle, standard.right) * standard.rotation;
      public void Flip() => mode = mode == Mode.ToA ? Mode.ToB : Mode.ToA;

      public void Process(Transform transform, Transform standard)
      {
        var target = targetQuat(standard);
        if (transform.rotation == target) { Flip(); }
        transform.rotation = Quaternion.RotateTowards(transform.rotation, target, speed);
      }
    }
    [Serializable]
    struct FollowSettings
    {
      public float speedFactor;
      public float speed => speedFactor * Time.deltaTime;
      public void Process(Transform transform, Transform target)
      {
        Quaternion targetRot = Quaternion.LookRotation(target.position - transform.position);
        if (targetRot == transform.rotation) { return; }
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, speed);
      }
    }

    [SerializeField] DroneAct _actor;
    public AiSight sight;

    [SerializeField] ReactiveProperty<AiVisible> _Target = new ReactiveProperty<AiVisible>();
    public EzLerp targettingProgress = new EzLerp(1);
    public AiVisible target
    {
      get { return _Target.Value; }
      private set { _Target.Value = value; }
    }
    public IObservable<AiVisible> Target => _Target;

    public Transform sightTransform;
    [SerializeField]
    SwipeSettings swipeSettings = new SwipeSettings
    {
      angleA = 10,
      angleB = 50,
      speedFactor = 10,
    };
    [SerializeField]
    FollowSettings followSettings = new FollowSettings
    {
      speedFactor = 20,
    };

    protected override void Blueprint()
    {
      sight.InSight
        .Subscribe(nextTarget =>
        {
          targettingProgress.SetMode(increase: nextTarget);
          if (nextTarget && !target)
          {
            _actor.reaction.Question();
          }
        });
      _actor.CameraUpdate(this)
        .Where(targettingProgress.isNeedsCalc)
        .Select(targettingProgress.UpdFactor)
        .Subscribe(fac =>
        {
          if (fac == 1 && !target)
          {
            target = sight.inSight;
            _actor.reaction.Exclamation();
          }
          if (fac == 0 && target)
          {
            target = null;
            _actor.reaction.GuruGuru();
          }
        });

      _actor.phase.ActivateSwitch(targets: this,
        cond: DronePhase.Patrol | DronePhase.Hostile | DronePhase.Attention | DronePhase.Standby);


      _actor.BehaviorUpdate(this)
        .Where(_actor.phase.IsAttension)
        .Subscribe(_ =>
        {
          _actor.MoveObjective(Vector3.zero);
          if (target || !sight.inSight)
          { _actor.phase.ShiftStandby(); }
        });

      _actor.CameraUpdate(this)
        .Subscribe(_ =>
          {
            switch (_actor.phase.current)
            {
              case DronePhase.Hostile:
                if (!target) { break; }
                followSettings.Process(
                  transform: sightTransform,
                  target: target.center);
                break;
              case DronePhase.Attention:
                if (!sight.inSight) { break; }
                followSettings.Process(
                  transform: sightTransform,
                  target: sight.inSight.center);
                break;
              case DronePhase.Patrol:
                swipeSettings.Process(
                  transform: sightTransform,
                  standard: _actor.transform);
                break;
            }
          });
    }
    void OnEnable() => Activate(true);
    void OnDisable() => Activate(false);
    void Activate(bool b)
    {
      // sight.enabled = b;
      sightTransform.gameObject.SetActive(b);
    }
  }
}
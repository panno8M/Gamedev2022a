using System;
using UnityEngine;
using UniRx;
using Senses.Sight;
using Assembly.GameSystem;

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

    public AiVisible target => sight.noticed;
    public IObservable<AiVisible> Target => sight.Noticed;

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
        .Subscribe(inSight =>
        {
          if (inSight && !sight.noticed)
          { _actor.reaction.Question(); }
        });
      sight.Noticed
        .Subscribe(noticed =>
        {
          if (noticed)
          { _actor.reaction.Exclamation(); }
          else
          { _actor.reaction.GuruGuru(); }
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
                if (!sight.noticed) { break; }
                followSettings.Process(
                  transform: sight.root,
                  target: target.center);
                break;
              case DronePhase.Attention:
                if (!sight.inSight) { break; }
                followSettings.Process(
                  transform: sight.root,
                  target: sight.inSight.center);
                break;
              case DronePhase.Patrol:
                swipeSettings.Process(
                  transform: sight.root,
                  standard: _actor.transform);
                break;
            }
          });
    }
    void OnEnable() => sight.ActivateOnce();
    void OnDisable() => sight.DisactivateOnce();
  }
}
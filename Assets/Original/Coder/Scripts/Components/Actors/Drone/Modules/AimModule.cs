using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using Senses.Sight;
using Assembly.GameSystem;

namespace Assembly.Components.Actors
{
  public class AimModule : DiBehavior
  {
    enum Mode { ToMin, ToMax, Reset }
    [SerializeField] DroneAct _actor;
    public AiSight sight;
    public Transform sightTransform;
    public float CamSwipeMin;
    public float CamSwipeMax;

    Quaternion defaultSightRotation;
    Mode mode;

    protected override void Blueprint()
    {
      defaultSightRotation = sightTransform.localRotation;

      sight.InSight
        .Where(_ => _actor.phase != DronePhase.Disactive)
        .Subscribe(visible => _actor.target = visible ? visible.center : null)
        .AddTo(this);

      _actor.OnPhaseEnter(DronePhase.Dead)
        .Subscribe(_ =>
        {
          sightTransform.gameObject.SetActive(false);
        });
      _actor.OnPhaseEnter(DronePhase.Patrol | DronePhase.Hostile)
        .Subscribe(phase =>
        {
          switch (phase)
          {
            case DronePhase.Patrol:
              mode = Mode.Reset;
              break;
          }
          sightTransform.gameObject.SetActive(true);
        });
      this.FixedUpdateAsObservable()
        .Where(x => this && isActiveAndEnabled)
        .Subscribe(_ =>
        {
          switch (_actor.phase)
          {
            case DronePhase.Patrol:
              switch (mode)
              {
                case Mode.Reset:
                  var rot = Quaternion.RotateTowards(sightTransform.localRotation, defaultSightRotation, 1f);
                  sightTransform.localRotation = rot;
                  if (rot == defaultSightRotation)
                  { mode = Mode.ToMin; }
                  break;
                default:
                  var deg = mode == Mode.ToMin ? CamSwipeMin : CamSwipeMax;
                  var target = Quaternion.AngleAxis(deg, transform.right) * transform.rotation;
                  var rot0 = Quaternion.RotateTowards(sightTransform.rotation, target, .1f);
                  sightTransform.rotation = rot0;
                  if (rot0 == target) { mode = mode == Mode.ToMin ? Mode.ToMax : Mode.ToMin; }
                  break;
              }
              break;


            case DronePhase.Hostile:
              if (_actor.target)
              {
                sightTransform.LookAt(_actor.target);
              }
              break;
          }
        });
    }
  }
}
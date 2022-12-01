using System;
using UnityEngine;

using UniRx;
using UniRx.Triggers;
using UniRx.Ex.ReactiveNeuro;
using Senses;
using Senses.Sight;

namespace Assembly.Components.Actors
{

  public class HostileDrone : MonoBehaviour
  {
    Context<Unit> droneContext = new Context<Unit>();

    IDisposable cancelBT;


    [SerializeField] float moveSpeed = 0.1f;
    [SerializeField] float rotateSpeed = 3f;
    [SerializeField] ParticleSystem psWater;
    [SerializeField] ParticleSystem psBurnUp;
    [SerializeField] AiSight sight;
    [SerializeField] DamagableComponent damagable;
    [SerializeField] Collider ctlTrigger;

    Tag reactableTag = Tag.CtlvolDroneMovable;
    [SerializeField] bool rotateTrigger;
    Quaternion rotateEnd;

    void Start()
    {
      sight.OnSeen
          .Subscribe(_ => psWater.Play()).AddTo(this);
      sight.OnLost
          .Subscribe(_ => psWater.Stop()).AddTo(this);

      damagable.TotalDamage
          .Where(total => total == 1)
          .Delay(TimeSpan.FromSeconds(0.5))
          .Subscribe(_ => psBurnUp.Play())
          .AddTo(this);

      damagable.OnBroken
          .Delay(TimeSpan.FromSeconds(0.5))
          .Subscribe(_ => Dead())
          .AddTo(this);

      cancelBT = BuildBihaviourPipeline(this.FixedUpdateAsObservable());

      ctlTrigger.OnTriggerExitAsObservable()
          .Where(other => other.gameObject.CompareTag(reactableTag.GetName()))
          .Subscribe(_ =>
          {
            rotateTrigger = true;
            rotateEnd = transform.rotation * Quaternion.AngleAxis(180f, Vector3.up);
          });

    }

    IDisposable BuildBihaviourPipeline<T>(IObservable<T> observable)
    {
      var priority = observable
          // TODO: Brain Subjectに担当させる ↓↓
          .Select(_ => droneContext)
          .Do(context =>
          {
            if (context.Status != TaskStatus.Running)
              context.Status = TaskStatus.Ready;
          })
          // ------------------------------- ↑↑
          .Multicast(new NeuroSubject<Unit>());

      priority
          .Where(context => sight.LastSeen.Value)
          .Subscribe(context =>
          {
            context.Status = TaskStatus.Success;
          });

      priority
          .ActivateIf(context => rotateTrigger)
          .Subscribe(context => RotateHalfSlowly(context));

      priority
          .Subscribe(context => MoveForward(context));

      return priority.Connect();
    }

    void MoveForward(Context<Unit> context)
    {
      transform.position -= transform.right * moveSpeed;
      context.Status = TaskStatus.Success;
    }
    void RotateHalfSlowly(Context<Unit> context)
    {
      context.Status = TaskStatus.Running;
      rotateTrigger = false;
      transform.rotation = Quaternion.RotateTowards(transform.rotation, rotateEnd, rotateSpeed);

      if (transform.rotation == rotateEnd)
      {
        context.Status = TaskStatus.Success;
      }
    }


    void Dead()
    {
      GetComponent<Rigidbody>().useGravity = true;
      GetComponent<Rigidbody>().isKinematic = false;

      cancelBT.Dispose();
      cancelBT = null;
    }
  }
}
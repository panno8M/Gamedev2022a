using System;
using UnityEngine;

using UniRx;
using UniRx.Triggers;
using UniRx.ReactiveNeuro;


public class HostileDrone : MonoBehaviour
{
    Context<Unit> droneContext = new Context<Unit>();

    [SerializeField] float moveSpeed = 0.1f;
    [SerializeField] float rotateSpeed = 3f;
    [SerializeField] ParticleSystem psWater;
    [SerializeField] AiSight sight;

    Layer lVolume = Layer.AiControlVolume;
    [SerializeField] bool rotateTrigger;
    Quaternion rotateEnd;

    void Start() {
        psWater.Stop();
        var slowupdate = this
            .UpdateAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(.5f))
            .Share();

       sight.OnSeen
            .Subscribe(_ => psWater.Play()).AddTo(this);
        sight.OnLost
            .Subscribe(_ => psWater.Stop()).AddTo(this);


        BuildBihaviourPipeline(this.FixedUpdateAsObservable());

        this.OnTriggerExitAsObservable()
            .Where(other => (int)lVolume == other.gameObject.layer)
            .Subscribe(_ => {
                rotateTrigger = true;
                rotateEnd = transform.rotation * Quaternion.AngleAxis(180f, Vector3.up);
            });

    }

    void BuildBihaviourPipeline<T>(IObservable<T> observable) {
        var priority = observable
            // TODO: Brain Subjectに担当させる ↓↓
            .Select(_ => droneContext)
            .Do(context => {
                if (context.Status != TaskStatus.Running)
                    context.Status = TaskStatus.Ready;})
            // ------------------------------- ↑↑
            .Multicast(new NeuroSubject<Unit>())
            .RefCount();

        priority
            .Where(context => sight.LastSeen.Value)
            .Subscribe(context => {
                context.Status = TaskStatus.Success;
            });

        priority
            .ActivateIf(context => rotateTrigger)
            .Subscribe(context => RotateHalfSlowly(context));

        priority
            .Subscribe(context => MoveForward(context));
    }

    void MoveForward(Context<Unit> context) {
        transform.position -= transform.right * moveSpeed;
        context.Status = TaskStatus.Success;
    }
    void RotateHalfSlowly(Context<Unit> context) {
        context.Status = TaskStatus.Running;
        rotateTrigger = false;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotateEnd, rotateSpeed);

        if (transform.rotation == rotateEnd) {
            context.Status = TaskStatus.Success;
        }
    }
}

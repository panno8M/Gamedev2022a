#if UNITY_EDITOR
// #define DEBUG_SIGNAL_LINE
#endif

using UnityEngine;
using UniRx;
using Assembly.GameSystem;
using Assembly.GameSystem.ObjectPool;


namespace Assembly.Components
{
  public class SignalLineParticle : DiBehavior, IPoolCollectable
  {
    public class CreateInfo : ObjectCreateInfo<SignalLineParticle>
    {
      public Vector3 targetPos;
      public Transform targetTrs;
      public override void Infuse(SignalLineParticle instance)
      {
        base.Infuse(instance);
        instance.targetPos = targetPos;
        instance.targetTrs = targetTrs;
      }
    }


#if DEBUG_SIGNAL_LINE
    [SerializeField]
#endif
    Vector3 targetPos;
#if DEBUG_SIGNAL_LINE
    [SerializeField]
#endif
    Transform targetTrs;

    [SerializeField] float speed;
    [SerializeField] float timeToDespawnOnTouch;
    float dtOnDone;
    bool done;

    Vector3 target => targetTrs?.position ?? targetPos;

    public IDespawnable despawnable { get; set; }
    [SerializeField] ParticleSystem _particle;
    public ParticleSystem particle => _particle ?? (_particle = GetComponent<ParticleSystem>());
    public void Assemble()
    {
      particle.Play();
    }
    public void Disassemble()
    {
      particle.Stop();
      dtOnDone = 0;
      done = false;
    }

    protected override void Blueprint()
    {
      Observable.EveryFixedUpdate()
          .Where(_ => isActiveAndEnabled)
          .Subscribe(_ => Move())
          .AddTo(this);
    }

    void Move()
    {
      if (done)
      {
        dtOnDone += Time.fixedDeltaTime;
        if (dtOnDone > timeToDespawnOnTouch) { despawnable.Despawn(); }
        return;
      }

      transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.fixedDeltaTime);
      if (transform.position == target)
      {
        particle.Stop();
        done = true;
      }
    }
  }
}
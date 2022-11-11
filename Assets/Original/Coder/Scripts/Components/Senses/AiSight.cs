using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace Assembly.Components.Senses
{
  public class AiSight : MonoBehaviour
  {
    public float eyesight = 20;
    public float sightCooldown = .5f;

    public Layer layerTarget = Layer.AiVisible;
    public Layer layerObstacle = Layer.Stage;
    public Layers lsEyeRay => new Layers(layerTarget, layerObstacle);
    public List<string> tags = new List<string>();

    public ReactiveProperty<AiVisible> LastSeen;
    public IObservable<AiVisible> OnSeen;
    public IObservable<AiVisible> OnLost;

    void Awake()
    {
      OnSeen = LastSeen
          .Where(x => x)
          .Share();
      OnLost = LastSeen
          .Pairwise()
          .Where(x => !x.Current)
          .Select(x => x.Previous)
          .Share();

      this.FixedUpdateAsObservable()
          .ThrottleFirst(TimeSpan.FromSeconds(sightCooldown))
          .Subscribe(_ => LookFor());
    }
    void Start()
    {
      OnSeen.Subscribe(visible => visible?.Find()).AddTo(this);
      OnLost.Subscribe(visible => visible?.Find(false)).AddTo(this);
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
      if (Physics.Raycast(ray, out hit, eyesight, lsEyeRay) && Find(hit))
      {
        Debug.DrawRay(ray.origin, ray.direction * eyesight, Color.blue, sightCooldown);
        LastSeen.Value = hit.collider.GetComponent<AiVisible>() ?? LastSeen.Value;
      }
      else
      {
        Debug.DrawRay(ray.origin, ray.direction * eyesight, Color.red, sightCooldown);
        LastSeen.Value = null;
      }
    }
  }
}
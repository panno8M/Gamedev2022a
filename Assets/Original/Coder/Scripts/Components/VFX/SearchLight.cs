using UnityEngine;
using Utilities;
using UniRx;
using UniRx.Triggers;
using Assembly.Components.StageGimmicks;
using Assembly.GameSystem;

namespace Assembly.Components
{
  public class SearchLight : DiBehavior
  {
    AlarmMgr alarmMgr;
    [Zenject.Inject]
    public void DepsInject(AlarmMgr alarmMgr)
    {
      this.alarmMgr = alarmMgr;
    }
    [SerializeField] new Light light;
    [SerializeField] MeshRenderer mesh;
    Material material;

    [SerializeField] Gradient gradient;

    [SerializeField] EzLerp progress;

    void Start() => Initialize();

    protected override void Blueprint()
    {
      material = mesh.materials[0];
      this.FixedUpdateAsObservable()
        .Where(progress.isNeedsCalc)
        .Select(progress.UpdFactor)
        .Subscribe(fac =>
        {
          Color color = gradient.Evaluate(fac);
          light.color = color;
          material.color = color;
        });
      alarmMgr.IsOnAlert.Subscribe(Switch);
    }
    public void Warn()
    {
      progress.SetAsIncrease();
      progress.SetFactor1();
    }
    public void Calm()
    {
      progress.SetAsDecrease();
    }
    public void Switch(bool b)
    {
      if (b) { Warn(); }
      else { Calm(); }
    }
  }
}
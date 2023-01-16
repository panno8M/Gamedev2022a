using UnityEngine;
using Utilities;
using UniRx;
using Assembly.GameSystem;
using Assembly.Components.StageGimmicks;

public class AlarmLight : DiBehavior
{
  AlarmMgr alarmMgr;
  [Zenject.Inject]
  public void DepsInject(AlarmMgr alarmMgr)
  {
    this.alarmMgr = alarmMgr;
  }
  [SerializeField] new Light light;

  [SerializeField] Gradient gradient;
  [SerializeField] float temperature;
  [SerializeField] float intensity;

  [SerializeField] float scrollSpeed = 1;

  [SerializeField] EzLerp transProgress;
#if DEBUG_ALARM_LIGHT
  [SerializeField]
#endif
  [Range(0, 1)] float alarmGradProgress;

  void Start() { Initialize(); }
  protected override void Blueprint()
  {
    if (!light) light = GetComponent<Light>();
    light.useColorTemperature = true;

    alarmMgr.IsOnAlert.Subscribe(b =>
    {
      transProgress.SetMode(increase: b);
      if (b) { transProgress.SetFactor1(); }
    });
  }
  void Update()
  {
    alarmGradProgress = Mathf.PingPong(Time.time * scrollSpeed, 1);

    transProgress.UpdFactor();
    light.intensity = transProgress.Add(0, intensity);
    light.colorTemperature = temperature;
    light.color = gradient.Evaluate(alarmGradProgress);
  }
}

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
  [SerializeField] bool alarm;
  [SerializeField] new Light light;

  [SerializeField] Color normalColor;
  [SerializeField] float normalTemperature;
  [SerializeField] float normalIntensity;

  [SerializeField] Gradient alarmColor;
  [SerializeField] float alarmTemperature;
  [SerializeField] float alarmIntensity;

  [SerializeField] float scrollSpeed = 1;

  [SerializeField] EzLerp transProgress;
  [SerializeField][Range(0, 1)] float alarmGradProgress;

  void Start() { Initialize(); }
  protected override void Blueprint()
  {
    if (!light) light = GetComponent<Light>();
    light.useColorTemperature = true;

    alarmMgr.IsOnAlert.Subscribe(b =>
    {
      transProgress.SetMode(increase: alarm = b);
      if (alarm) { transProgress.SetFactor1(); }
    });
  }
  void Update()
  {
    alarmGradProgress = Mathf.PingPong(Time.time * scrollSpeed, 1);

    transProgress.UpdFactor();
    light.intensity = transProgress.Mix(normalIntensity, alarmIntensity);
    light.colorTemperature = transProgress.Mix(normalTemperature, alarmTemperature);
    light.color = transProgress.Mix(normalColor, alarmColor.Evaluate(alarmGradProgress));
  }
}

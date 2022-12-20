using UnityEngine;
using Utilities;
using UniRx;

public class AlarmLight : MonoBehaviour
{
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
  void Start()
  {
    if (!light) light = GetComponent<Light>();
    light.useColorTemperature = true;

    AlarmMgr.Instance.IsOnAlert.Subscribe(b =>
    {
      transProgress.SetAsIncrease(alarm = b);
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

using UnityEngine;
using UniRx;
using Cinemachine;
using Assembly.GameSystem;

namespace Assembly.Components
{
  [RequireComponent(typeof(SafetyTrigger))]
  public class CamctlOverride : MonoBehaviour
  {
    public enum OverrideMode { Temporary, Forever }
    [SerializeField] CinemachineVirtualCamera _cmCamera;
    [SerializeField] OverrideMode _overrideMode;
    [SerializeField] bool followEncounter;

    SafetyTrigger trigger;

    void Awake()
    {
      trigger = GetComponent<SafetyTrigger>();

      trigger.OnEnter
          .Where(other => other.CompareTag(Tag.CtlvolCamera.GetName()))
          .Subscribe(other =>
          {
            if (followEncounter)
            { _cmCamera.m_Follow = other.transform; }

            _cmCamera.enabled = true;
          });

      trigger.OnExit
        .Where(other => _overrideMode == OverrideMode.Temporary)
        .Where(other => other.CompareTag(Tag.CtlvolCamera.GetName()))
        .Subscribe(other => _cmCamera.enabled = false);
    }
  }
}
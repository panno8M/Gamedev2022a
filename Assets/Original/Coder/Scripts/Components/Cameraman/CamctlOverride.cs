using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Cinemachine;

using Assembly.Components;
public class CamctlOverride : MonoBehaviour
{
  public enum OverrideMode { Temporary, Forever }
  [SerializeField] CinemachineVirtualCamera _cmCamera;
  [SerializeField] OverrideMode _overrideMode;
  [SerializeField] int priority = 20;

  void Awake()
  {
    this.OnTriggerEnterAsObservable()
        .Where(other => other.CompareTag(Tag.CtlvolCamera.GetName()))
        .Subscribe(other =>
        {
          _cmCamera.m_Follow = Global.Player.transform;
          _cmCamera.Priority = priority;
        });
    this.OnTriggerExitAsObservable()
        .Where(other => _overrideMode == OverrideMode.Temporary)
        .Where(other => other.CompareTag(Tag.CtlvolCamera.GetName()))
        .Subscribe(other =>
        {
          _cmCamera.Priority = 0;
          _cmCamera.m_Follow = null;
        });

  }
}
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Cinemachine;
using Assembly.GameSystem;
using Assembly.Components.Pools;

namespace Assembly.Components
{
  public class CamctlOverride : MonoBehaviour
  {
    PlayerPool playerPool;
    [Zenject.Inject]
    public void DepsInject(PlayerPool playerPool)
    {
      this.playerPool = playerPool;
    }

    public enum OverrideMode { Temporary, Forever }
    [SerializeField] CinemachineVirtualCamera _cmCamera;
    [SerializeField] OverrideMode _overrideMode;
    [SerializeField] int priority = 20;
    [SerializeField] bool followPlayer;

    void Awake()
    {
      this.OnTriggerEnterAsObservable()
          .Where(other => other.CompareTag(Tag.CtlvolCamera.GetName()))
          .Subscribe(other =>
          {
            if (followPlayer)
            {
              _cmCamera.m_Follow = playerPool.player.transform;
            }
            _cmCamera.Priority = priority;
          });
      this.OnTriggerExitAsObservable()
          .Where(other => _overrideMode == OverrideMode.Temporary)
          .Where(other => other.CompareTag(Tag.CtlvolCamera.GetName()))
          .Subscribe(other =>
          {
            _cmCamera.Priority = 0;
            if (followPlayer && _cmCamera.m_Follow == playerPool.player.transform)
            {
              _cmCamera.m_Follow = null;
            }
          });

      playerPool.player.OnAssembleObservable
          .Subscribe(_ =>
          {
            _cmCamera.Priority = 0;
          }).AddTo(this);

    }
  }
}
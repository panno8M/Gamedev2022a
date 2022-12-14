using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Cinemachine;
using Assembly.GameSystem;
using Assembly.Components.Actors.Player;

namespace Assembly.Components
{
  public class CamctlOverride : MonoBehaviour
  {
    PlayerAct player;
    [Zenject.Inject]
    public void DepsInject(PlayerAct player)
    {
      this.player = player;
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
              _cmCamera.m_Follow = player.transform;
            }
            _cmCamera.Priority = priority;
          });
      this.OnTriggerExitAsObservable()
          .Where(other => _overrideMode == OverrideMode.Temporary)
          .Where(other => other.CompareTag(Tag.CtlvolCamera.GetName()))
          .Subscribe(other =>
          {
            _cmCamera.Priority = 0;
            if (followPlayer && _cmCamera.m_Follow == player.transform)
            {
              _cmCamera.m_Follow = null;
            }
          });

      player.OnAssembleObservable
          .Subscribe(_ =>
          {
            _cmCamera.Priority = 0;
          }).AddTo(this);

    }
  }
}
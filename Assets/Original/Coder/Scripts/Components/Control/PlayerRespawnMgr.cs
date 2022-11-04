using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayerRespawnMgr : UniqueBehaviour<PlayerRespawnMgr> {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init() { instance = null; }

    [SerializeField] GameObject _prefPlayer;
    public Transform activeSpawnPoint;

    public Subject<Unit> _respawnRequest = new Subject<Unit>();
    public IObserver<Unit> RespawnRequest => _respawnRequest;

    void Start() {
        _respawnRequest
            .StartWith(Unit.Default)
            .Subscribe(_ => {
                if (Global.Player == null) {
                    Instantiate(_prefPlayer, activeSpawnPoint.position, Quaternion.identity);
                    Global.Player.name = _prefPlayer.name;
                } else {
                    Global.Player.transform.position = activeSpawnPoint.position;
                }
            }).AddTo(this);

    }

}

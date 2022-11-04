using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawnMgr : UniqueBehaviour<PlayerRespawnMgr> {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init() { instance = null; }

    [SerializeField] GameObject _prefPlayer;
    public Transform activeSpawnPoint;

    void Start() {
        if (Global.Player == null) {
            Instantiate(_prefPlayer, activeSpawnPoint.position, Quaternion.identity);
            Global.Player.name = _prefPlayer.name;
        }
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UniRx.Toolkit;

public class PlayerPool : ObjectPool<Player> {
    public GameObject prefab;
    public Transform spawnAt;
    public PlayerPool(GameObject prefab, Transform spawnAt) {
        this.prefab = prefab;
        this.spawnAt = spawnAt;
    }
    protected override Player CreateInstance() {
        if (Global.Player != null) { return Global.Player; }
        var result = GameObject.Instantiate(prefab, spawnAt.position, Quaternion.identity);
        result.name = prefab.name;
        return result.GetComponent<Player>();
    }
    protected override void OnBeforeRent(Player instance) {
        base.OnBeforeRent(instance);
        instance.Damagable.Repair();
        instance.transform.position = spawnAt.position;
    }
}

public class PlayerRespawnMgr : UniqueBehaviour<PlayerRespawnMgr> {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init() { instance = null; }

    [SerializeField] GameObject _prefPlayer;
    public Transform activeSpawnPoint;

    PlayerPool pool;

    void Awake() {
        pool = new PlayerPool(_prefPlayer, activeSpawnPoint);
        this.OnDestroyAsObservable()
            .Subscribe(_ => pool.Dispose());
        Respawn();
    }

    void Start() {

        Global.Player.Damagable.OnBroken
            .Delay(TimeSpan.FromSeconds(0.5f))
            .Subscribe(_ => Respawn());

        Global.Player.Damagable.OnBroken
            .Subscribe(_ => pool.Return(Global.Player))
            .AddTo(this);
    }

    public void Respawn() { pool.Rent(); }

}

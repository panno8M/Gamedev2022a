using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UniRx.Toolkit;

namespace Assembly.Components.Actors
{
  public class PlayerPool : ObjectPool<PlayerAct>
  {
    public GameObject prefab;
    public Transform spawnAt;
    public PlayerPool(GameObject prefab, Transform spawnAt)
    {
      this.prefab = prefab;
      this.spawnAt = spawnAt;
    }
    protected override PlayerAct CreateInstance()
    {
      if (Global.Player != null) { return Global.Player; }
      var result = GameObject.Instantiate(prefab, spawnAt.position, Quaternion.identity);
      result.name = prefab.name;
      return result.GetComponent<PlayerAct>();
    }
  }

  public class PlayerRespawnMgr : UniqueBehaviour<PlayerRespawnMgr>
  {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init() { instance = null; }

    [SerializeField] GameObject _prefPlayer;
    public Transform activeSpawnPoint;

    PlayerPool _pool;
    PlayerPool pool => _pool ?? (_pool = new PlayerPool(_prefPlayer, activeSpawnPoint));

    BehaviorSubject<PlayerAct> _OnSpawn = new BehaviorSubject<PlayerAct>(null);
    public IObservable<PlayerAct> OnSpawn => _OnSpawn.Where(x => x);

    public PlayerAct Rent()
    {
      var result = pool.Rent();
      if (result) { _OnSpawn.OnNext(result); }
      return result;
    }
    public void Return()
    {
      pool.Return(Global.Player);
    }

    void Awake()
    {
      this.OnDestroyAsObservable()
          .Subscribe(_ => pool.Dispose());
      Rent();
    }

    public PlayerAct RequestRespawn() { return Rent(); }

  }
}
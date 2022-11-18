using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UniRx.Toolkit;

namespace Assembly.Components.Actors.Player.Pure
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
    protected override void OnBeforeRent(PlayerAct instance)
    {
      base.OnBeforeRent(instance);
      instance.Damagable.Repair();
      instance.transform.position = spawnAt.position;
    }
    protected override void OnBeforeReturn(PlayerAct instance)
    {
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

    BehaviorSubject<PlayerAct> _OnRent = new BehaviorSubject<PlayerAct>(null);
    public IObservable<PlayerAct> OnRent => _OnRent.Where(x => x);
    Subject<PlayerAct> _OnReturn = new Subject<PlayerAct>();
    public IObservable<PlayerAct> OnReturn => _OnReturn;

    public PlayerAct Rent() {
      var result = pool.Rent();
      if (result) { _OnRent.OnNext(result); }
      return result;
    }
    public void Return() {
      if (Global.Player) { _OnReturn.OnNext(Global.Player); }
      pool.Return(Global.Player);
    }

    void Awake()
    {
      this.OnDestroyAsObservable()
          .Subscribe(_ => pool.Dispose());
      Rent();
    }

    void Start()
    {
      OnReturn
        .Delay(TimeSpan.FromMilliseconds(2000))
        .Subscribe(_ => Rent());
    }

    public PlayerAct RequestRespawn() { return Rent(); }

  }
}
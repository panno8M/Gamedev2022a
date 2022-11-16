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

    public PlayerAct Rent() {
      var result = pool.Rent();
      if (result) { _OnRent.OnNext(result); }
      return result;
    }

    void Awake()
    {
      this.OnDestroyAsObservable()
          .Subscribe(_ => pool.Dispose());
      Respawn();
    }

    void Start()
    {

      Global.Player.Damagable.OnBroken
          .Delay(TimeSpan.FromSeconds(0.5f))
          .Subscribe(_ => Respawn());

      Global.Player.Damagable.OnBroken
          .Subscribe(_ => pool.Return(Global.Player))
          .AddTo(this);
    }

    public PlayerAct Respawn() { return Rent(); }

  }
}
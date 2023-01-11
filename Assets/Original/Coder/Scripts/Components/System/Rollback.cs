#if UNITY_EDITOR
// #define DEBUG_ROLLBACK
#endif

using System;
using UnityEngine;
using UniRx;

namespace Assembly.Components
{
  public class Rollback : MonoBehaviour
  {
#if DEBUG_ROLLBACK
    [Header("[Debug Inspector]\ndon't forget to turn symbol DEBUG_ROLLBACK off.")]
    byte __headerTarget__;
#endif
    public void Preflight(IRollbackDispatcher dispatcher)
    {
#if DEBUG_ROLLBACK
      Debug.Log($"{name}: Preflight");
#endif
      _Preflight.OnNext(dispatcher);
    }
    public void Execute(IRollbackDispatcher dispatcher)
    {
#if DEBUG_ROLLBACK
      Debug.Log($"{name}: Execute");
#endif
      _Executtion.OnNext(dispatcher);
    }

    public void UpdateTransaction(ITransactionDispatcher dispatcher)
    {
      _Transaction.OnNext(dispatcher);
    }

    Subject<IRollbackDispatcher> _Preflight = new Subject<IRollbackDispatcher>();
    public IObservable<IRollbackDispatcher> OnPreflight => _Preflight;

    Subject<IRollbackDispatcher> _Executtion = new Subject<IRollbackDispatcher>();
    public IObservable<IRollbackDispatcher> OnExecute => _Executtion;

    Subject<ITransactionDispatcher> _Transaction = new Subject<ITransactionDispatcher>();
    public IObservable<ITransactionDispatcher> OnUpdateTransaction => _Transaction;
  }

  public interface IRollbackDispatcher
  {
  }
  public interface ITransactionDispatcher
  {
  }
}
using System;
using UnityEngine;
using UniRx;

namespace Assembly.Components
{
  public class Rollback : MonoBehaviour
  {
    public void Execute(IRollbackDispatcher dispatcher)
    {
      _Executtion.OnNext(dispatcher);
    }

    public void UpdateTransaction(ITransactionDispatcher dispatcher)
    {
      _Transaction.OnNext(dispatcher);
    }

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
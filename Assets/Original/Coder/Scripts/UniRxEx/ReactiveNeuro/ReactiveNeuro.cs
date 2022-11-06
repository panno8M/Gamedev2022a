using System;

namespace UniRx.Ex.ReactiveNeuro
{
  public class Brain<T>
  {
    public Subject<Context<T>> Root;

    public Brain()
    {
      Root = new Subject<Context<T>>();
    }
  }

  public class Context<T>
  {
    TaskStatus _status;
    IObserver<Context<T>> _runningExecution;
    Context<T> _runningContext;

    public TaskStatus Status
    {
      get => _status;
      set
      {
        _status = value;
        if (value != TaskStatus.Running)
        {
          _runningExecution = null;
          _runningContext = null;
        }
      }
    }

    public TaskStatus Try(IObserver<Context<T>> observer, Context<T> context)
    {
      context._status = TaskStatus.Ready;
      observer.OnNext(context);
      if (context._status == TaskStatus.Running)
      {
        _runningExecution = observer;
        _runningContext = context;
      }
      _status = context._status;
      return context.Status;
    }
    public bool TryContinue()
    {
      if (_status != TaskStatus.Running) return false;
      if (_runningExecution == null || _runningContext == null) return false;

      _runningExecution.OnNext(_runningContext);
      Status = _runningContext._status;
      return true;
    }
  }

  public enum TaskStatus
  {
    Ready, Success, Failure, Running
  }

}

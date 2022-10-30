using System;
using System.Collections;
using System.Collections.Generic;

namespace UniRx.ReactiveNeuro
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
        public TaskStatus Status;
        public IObserver<Context<T>> TryingExecution;
    }

    public enum TaskStatus
    {
        Ready, Success, Failure, Running
    }

}

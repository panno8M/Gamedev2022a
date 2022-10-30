using System;
using System.Collections;
using System.Collections.Generic;
using UniRx.InternalUtil;

namespace UniRx.ReactiveNeuro {
    public sealed class NeuroSubject<T> : ISubject<Context<T>>, IDisposable, IOptimizedObservable<Context<T>> {
        object observerLock = new object();

        bool isStopped;
        bool isDisposed;
        Exception lastError;
        IObserver<Context<T>> outObserver = EmptyObserver<Context<T>>.Instance;

        Context<T> _context = new Context<T>();

        public bool HasObservers {
            get {
                return !(outObserver is EmptyObserver<Context<T>>) && !isStopped && !isDisposed;
            }
        }

        public void OnCompleted() {
            IObserver<Context<T>> old;
            lock (observerLock) {
                ThrowIfDisposed();
                if (isStopped) return;

                old = outObserver;
                outObserver = EmptyObserver<Context<T>>.Instance;
                isStopped = true;
            }

            old.OnCompleted();
        }

        public void OnError(Exception error) {
            if (error == null) throw new ArgumentNullException("error");

            IObserver<Context<T>> old;
            lock (observerLock) {
                ThrowIfDisposed();
                if (isStopped) return;

                old = outObserver;
                outObserver = EmptyObserver<Context<T>>.Instance;
                isStopped = true;
                lastError = error;
            }

            old.OnError(error);
        }

        public void OnNext(Context<T> value) {
            //_context = value;
            _context.Status = value.Status;
            outObserver.OnNext(_context);
            value.Status = _context.Status;
        }

        public IDisposable Subscribe(IObserver<Context<T>> observer) {
            if (observer == null) throw new ArgumentNullException("observer");

            var ex = default(Exception);

            lock (observerLock) {
                ThrowIfDisposed();
                if (!isStopped) {
                    var listObserver = outObserver as NeuroObserver<T>;
                    if (listObserver != null) {
                        outObserver = listObserver.Add(observer);
                    }
                    else {
                        var current = outObserver;
                        outObserver = new PriorityObserver<T>(new ImmutableList<IObserver<Context<T>>>(new[] { current, observer }));
                    }

                    return new Subscription(this, observer);
                }

                ex = lastError;
            }

            if (ex != null) {
                observer.OnError(ex);
            }
            else {
                observer.OnCompleted();
            }

            return Disposable.Empty;
        }

        public void Dispose() {
            lock (observerLock) {
                isDisposed = true;
                outObserver = DisposedObserver<Context<T>>.Instance;
            }
        }

        void ThrowIfDisposed() {
            if (isDisposed) throw new ObjectDisposedException("");
        }

        public bool IsRequiredSubscribeOnCurrentThread() {
            return false;
        }

        class Subscription : IDisposable {
            readonly object gate = new object();
            NeuroSubject<T> parent;
            IObserver<Context<T>> unsubscribeTarget;

            public Subscription(NeuroSubject<T> parent, IObserver<Context<T>> unsubscribeTarget) {
                this.parent = parent;
                this.unsubscribeTarget = unsubscribeTarget;
            }

            public void Dispose() {
                lock (gate) {
                    if (parent == null) { return; }
                    lock (parent.observerLock) {
                        var listObserver = parent.outObserver as PriorityObserver<T>;
                        if (unsubscribeTarget == parent._context.TryingExecution) {
                            parent._context.Status = TaskStatus.Failure;
                        }
                        if (listObserver != null) {
                            parent.outObserver = listObserver.Remove(unsubscribeTarget);
                        }
                        else {
                            parent.outObserver = EmptyObserver<Context<T>>.Instance;
                        }

                        unsubscribeTarget = null;
                        parent = null;
                    }
                }
            }
        }
    }


    public class NeuroObserver<T> : IObserver<Context<T>> {
        protected readonly ImmutableList<IObserver<Context<T>>> _observers;

        public virtual NeuroObserver<T> Create(ImmutableList<IObserver<Context<T>>> observers) {
            return new NeuroObserver<T>(observers);
        }

        public NeuroObserver(ImmutableList<IObserver<Context<T>>> observers) {
            _observers = observers;
        }

        public void OnCompleted() {
            var targetObservers = _observers.Data;
            for (int i = 0; i < targetObservers.Length; i++) {
                targetObservers[i].OnCompleted();
            }
        }

        public void OnError(Exception error) {
            var targetObservers = _observers.Data;
            for (int i = 0; i < targetObservers.Length; i++) {
                targetObservers[i].OnError(error);
            }
        }

        public virtual void OnNext(Context<T> value){}
        internal IObserver<Context<T>> Add(IObserver<Context<T>> observer) {
            return Create(_observers.Add(observer));
        }

        internal IObserver<Context<T>> Remove(IObserver<Context<T>> observer) {
            var i = Array.IndexOf(_observers.Data, observer);
            if (i < 0)
                return this;

            if (_observers.Data.Length == 2) {
                return _observers.Data[1 - i];
            }
            else {
                return Create(_observers.Remove(observer));
            }
        }
    }
    public class PriorityObserver<T> : NeuroObserver<T> {
        public PriorityObserver(ImmutableList<IObserver<Context<T>>> observers)
            : base(observers) {}

        public override NeuroObserver<T> Create(ImmutableList<IObserver<Context<T>>> observers) {
            return new PriorityObserver<T>(observers) as NeuroObserver<T>;
        }

        public override void OnNext(Context<T> value) {
            if (value.Status == TaskStatus.Running) {
                value.TryingExecution.OnNext(value);
                return;
            }
            var targetObservers = _observers.Data;
            for (int i = 0; i < targetObservers.Length; i++) {
                value.TryingExecution = targetObservers[i];
                targetObservers[i].OnNext(value);

                switch (value.Status){
                    case TaskStatus.Ready:
                    case TaskStatus.Failure:
                        break;
                    case TaskStatus.Success:
                    case TaskStatus.Running:
                        return;
                }
            }
        }
    }

    public class SequenceObserver<T> : NeuroObserver<T> {
        public SequenceObserver(ImmutableList<IObserver<Context<T>>> observers)
            : base(observers) {}

        public override NeuroObserver<T> Create(ImmutableList<IObserver<Context<T>>> observers) {
            return new SequenceObserver<T>(observers) as NeuroObserver<T>;
        }

        public override void OnNext(Context<T> value) {
            if (value.Status == TaskStatus.Running) {
                value.TryingExecution.OnNext(value);
                return;
            }
            var targetObservers = _observers.Data;
            for (int i = 0; i < targetObservers.Length; i++) {
                value.TryingExecution = targetObservers[i];
                targetObservers[i].OnNext(value);

                switch (value.Status){
                    case TaskStatus.Ready:
                    case TaskStatus.Success:
                        break;
                    case TaskStatus.Failure:
                    case TaskStatus.Running:
                        return;
                }
            }
        }
    }

    public static class NeuroObservableExtensions {
        public static IObservable<Context<T>> ActivateIf<T>(this IObservable<Context<T>> source, Func<Context<T>, bool> predicate) {
            return source.Where(value => predicate(value) || value.Status == TaskStatus.Running);
        }
    }
}

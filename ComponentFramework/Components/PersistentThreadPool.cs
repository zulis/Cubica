using System;
using System.Collections.Generic;
using System.Threading;
using ComponentFramework.Core;

namespace ComponentFramework.Components
{
    public class PersistentThreadPool : Component, IThreadPoolService
    {
        const int InitialMaxThreads = 1;
        readonly Stack<PersistentThread> stack;

        public PersistentThreadPool(ICore core) : base(core)
        {
            stack = new Stack<PersistentThread>(InitialMaxThreads);

            for (int i = 0; i < InitialMaxThreads; i++)
                stack.Push(new PersistentThread());
        }

        public Worker Take(Action task)
        {
            var thread = stack.Count > 0 ? stack.Pop() : new PersistentThread();
            return new Worker(thread, task);
        }
        public Worker<TContext> Take<TContext>(Action<TContext> task)
        {
            var thread = stack.Count > 0 ? stack.Pop() : new PersistentThread();
            return new Worker<TContext>(thread, task);
        }
        public Worker<TContext, TResult> Take<TContext, TResult>(Func<TContext, TResult> task)
        {
            var thread = stack.Count > 0 ? stack.Pop() : new PersistentThread();
            return new Worker<TContext, TResult>(thread, task);
        }

        public void Return(WorkerBase worker)
        {
            worker.Dispose();
            if (Disposed)
                worker.UnderlyingThread.Dispose();
            else
                stack.Push(worker.UnderlyingThread);
        }

        protected override void DisposeInternal()
        {
            while (stack.Count > 0)
                stack.Pop().Dispose();
        }
    }

    public interface IThreadPoolService : IService
    {
        Worker Take(Action task);
        Worker<TContext> Take<TContext>(Action<TContext> task);
        Worker<TContext, TResult> Take<TContext, TResult>(Func<TContext, TResult> task);

        void Return(WorkerBase thread);
    }

    class PersistentThread : IDisposable
    {
        readonly Thread thread;
        readonly ManualResetEvent startEvent, joinEvent;

        public bool Started { get; private set; }
        public bool Disposed { get; private set; }

        public IWorker CurrentWorker { private get; set; }

        public PersistentThread()
        {
            startEvent = new ManualResetEvent(false);
            joinEvent = new ManualResetEvent(false);

            thread = new Thread(DoWork);
            thread.Start();
        }

        public void Start()
        {
            Started = true;

            startEvent.Set();
        }

        public void Join()
        {
            joinEvent.WaitOne();
            joinEvent.Reset();

            Started = false;
        }

        void DoWork()
        {
            startEvent.WaitOne();
            startEvent.Reset();

            while (!Disposed)
            {
                CurrentWorker.Act();

                joinEvent.Set();
                startEvent.WaitOne();
                startEvent.Reset();
            }
        }

        public ThreadPriority Priority
        {
            set { thread.Priority = value; }
        }

        public void Dispose()
        {
            if (!Disposed)
                GC.SuppressFinalize(this);

            DisposeInternal();
        }

        void DisposeInternal()
        {
            if (!Disposed)
            {
                Disposed = true;
                startEvent.Set();
            }
        }

        ~PersistentThread()
        {
            DisposeInternal();
        }
    }

    interface IWorker
    {
        void Act();
    }

    public class Worker : WorkerBase
    {
        readonly Action task;

        internal Worker(PersistentThread thread, Action task) : base(thread)
        {
            this.task = task;
        }

        public override void Act()
        {
            task();
        }

        public void Start()
        {
            if (thread.Started)
                throw new InvalidOperationException("Thread is already started");
            if (thread.Disposed)
                throw new ObjectDisposedException("PersistentThread");

            thread.CurrentWorker = this;

            thread.Start();
        }
    }

    public class Worker<TContext> : WorkerBase
    {
        readonly Action<TContext> task;

        TContext cachedContext;

        internal Worker(PersistentThread thread, Action<TContext> task) : base(thread)
        {
            this.task = task;
        }

        public override void Act()
        {
            task(cachedContext);
        }

        public void Start(TContext context)
        {
            if (thread.Started)
                throw new InvalidOperationException("Thread is already started");
            if (thread.Disposed)
                throw new ObjectDisposedException("PersistentThread");

            cachedContext = context;
            thread.CurrentWorker = this;

            thread.Start();
        }
    }

    public class Worker<TContext, TResult> : WorkerBase
    {
        readonly Func<TContext, TResult> task;

        TContext cachedContext;
        TResult result;

        internal Worker(PersistentThread thread, Func<TContext, TResult> task) : base(thread)
        {
            this.task = task;
        }

        public override void Act()
        {
            result = task(cachedContext);
        }

        public void Start(TContext context)
        {
            if (thread.Started)
                throw new InvalidOperationException("Thread is already started");
            if (thread.Disposed)
                throw new ObjectDisposedException("PersistentThread");

            cachedContext = context;
            thread.CurrentWorker = this;

            thread.Start();
        }

        public TResult Result
        {
            get
            {
                if (thread.Started)
                    Join();
                return result;
            } 
        }
    }

    public abstract class WorkerBase : IWorker
    {
        readonly internal PersistentThread thread;

        internal WorkerBase(PersistentThread thread)
        {
            this.thread = thread;
        }

        internal void Dispose()
        {
            thread.Priority = ThreadPriority.Normal;
        }

        public ThreadPriority Priority
        {
            set { thread.Priority = value; }
        }

        internal PersistentThread UnderlyingThread
        {
            get { return thread; }
        }

        public abstract void Act();

        public void Join()
        {
            if (!thread.Started)
                // Idle thread, no join needed
                return;

            if (thread.Disposed)
                throw new ObjectDisposedException("PersistentThread");

            thread.Join();
        }
    }
}

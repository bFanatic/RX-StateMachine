using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Core.Queue
{
    public abstract class StateMachine // : EventWaitHandle
    {
        #region Delegates

        public delegate void OnStateChange(QueueStates state);

        public delegate void OnTrace(string message);

        #endregion

        private IDisposable StateMachineEngine;

        protected Timer WatchDog;

        protected StateMachine()
           // : base(true, EventResetMode.AutoReset)
        {
            WatchDog = new Timer();
            WatchDog.Elapsed += WatchDog_Elapsed;
        }

        protected bool Initialized { get; set; }
        public event OnStateChange StateChanged;
        public event OnTrace Trace;

        public void Start()
        {
            IObservable<QueueStates> states = Observable.Create<QueueStates>(o => QueueMechanism(o));
            StateMachineEngine = states.Subscribe(OutputStateChange);
        }

        public void Stop()
        {
            if (StateMachineEngine == null) return;

            StateMachineEngine.Dispose();

            TraceMessage("** Machine stop requested **");
        }

        protected void TraceMessage(string message)
        {
            if (Trace != null)
                Trace.Invoke(message);
        }


        private void OutputStateChange(QueueStates state)
        {
            if (StateChanged == null) return;

            StateChanged.Invoke(state);
        }

        private CancellationDisposable QueueMechanism(IObserver<QueueStates> observer)
        {
            var cancellation = new CancellationDisposable();
            // internally creates a new CancellationTokenSource

            NewThreadScheduler.Default.Schedule((t) =>
                                                    {
                                                        QueueStates state = QueueStates.StartUp;
                                                        for (; ; )
                                                        {
                                                            bool success = ExecuteStateLogic(state, cancellation);

                                                            if (cancellation.Token.IsCancellationRequested)
                                                            {
                                                                state = state == QueueStates.Teardown
                                                                            ? GetNextState(state)
                                                                            : QueueStates.Teardown;
                                                            }
                                                            
                                                            else if (success)
                                                                state = GetNextState(state);
                                                            else
                                                                state = QueueStates.Error;

                                                            if (state == QueueStates.WaitingForTrigger)
                                                            {
                                                            }
                                                            else if (state != QueueStates.Stopped)
                                                                observer.OnNext(state);
                                                            else
                                                            {
                                                                TraceMessage("--**-- Stopped --**--");
                                                                observer.OnCompleted();
                                                                return;
                                                            }

                                                            //// here we do the long lasting background operation
                                                            //if (!cancellation.Token.IsCancellationRequested)
                                                            //{
                                                            //    // check cancel token periodically

                                                            //}
                                                            //else
                                                            //{
                                                            //    TraceMessage("Aborting because cancel event was signaled!");
                                                            //    observer.OnCompleted();
                                                            //    return;
                                                            //}
                                                        }
                                                    }
                );

            return cancellation;
        }

        private bool ExecuteStateLogic(QueueStates state, CancellationDisposable cancellation)
        {
            switch (state)
            {
                case QueueStates.StartUp:
                    return true;

                case QueueStates.Initializing:
                    return Initialize();

                case QueueStates.WaitingForTrigger:
                    return true;

                case QueueStates.Executing:
                    return BaseExecute(cancellation);
                    
                case QueueStates.Teardown:
                    return Teardown();

                case QueueStates.Stopped:
                    // not used
                    return true;

                case QueueStates.Error:
                    OnError("-- illogical flow --");
                    return true;

                default:
                    return false;
            }
        }

        private QueueStates GetNextState(QueueStates state)
        {
            switch (state)
            {
                case QueueStates.StartUp:
                    return QueueStates.Initializing;

                case QueueStates.Initializing:
                    return QueueStates.WaitingForTrigger;

                case QueueStates.WaitingForTrigger:
                    return QueueStates.Executing;

                case QueueStates.Executing:
                    return QueueStates.WaitingForTrigger;

                case QueueStates.Teardown:
                    return QueueStates.Stopped;

                case QueueStates.Error:
                default:
                    return QueueStates.Initializing;
            }
        }

        private bool BaseExecute(CancellationDisposable cancellation)
        {
            //start timer
            if (WatchDog != null)
            {
                WatchDog.Enabled = true;
                WatchDog.Start();
            }

            bool success = Execute(cancellation);

            if (WatchDog != null)
            {
                WatchDog.Enabled = false;
                WatchDog.Stop();
            }

            return success;
        }

        protected abstract bool Initialize();
        protected abstract bool Execute(CancellationDisposable cancellation);

        protected abstract bool Teardown();
        protected abstract bool OnError(string message);

        private void WatchDog_Elapsed(object sender, ElapsedEventArgs e)
        {
            var t = sender as Timer;
            if (t != null)
                t.Enabled = false;

            OnError(string.Format("watchdog elapsed - {0}", e.SignalTime));
        }

        #region Nested type: MessageToken

        public class MessageToken
        {
            public CancellationDisposable Cancellation;
            public BooleanDisposable InErrorState;

            public MessageToken()
            {
                Cancellation = new CancellationDisposable();
                InErrorState = new BooleanDisposable();
            }
        }

        #endregion
    }

    public class TestMachine : StateMachine
    {
        protected override bool Initialize()
        {
            if (!Initialized)
            {
                TraceMessage("Initialize");
                Thread.Sleep(1000);

                WatchDog.Interval = 1000;
            }
            else
                TraceMessage("Allready initialized");


            return true;
        }

        protected override bool Execute(CancellationDisposable cancellation)
        {
            TraceMessage("Execute");

            Thread.Sleep(3000);
            return true;
        }

        protected override bool Teardown()
        {
            TraceMessage("Teardown");
            Thread.Sleep(200);
            TraceMessage("Teardown - doing stuff - 001");
            Thread.Sleep(200);
            TraceMessage("Teardown - doing stuff - 002");
            Thread.Sleep(300);
            TraceMessage("Teardown - doing stuff - 003");
            Thread.Sleep(500);
            TraceMessage("Teardown - doing stuff - 004");

            return true;
        }

        protected override bool OnError(string message)
        {
            TraceMessage(string.Format("{0} - {1}", "OnError", message));
            return true;
        }
    }
}
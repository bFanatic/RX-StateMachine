using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Core.StateM
{
    public class BasicMachine
    {
        private Timer _timedExecutionTimer;
        private readonly CancellationTokenSource _cancellation = new CancellationTokenSource();
        private BaseExecutor _executor;

        public BasicMachine()
        {
            BusinessStateLogic.SignalMessageEvent += SignalMessage;
        }

        private BaseExecutor BusinessStateLogic
        {
            get { return _executor ?? (_executor = new ConcreteExecutor()); }
        }

        private void _timedExecutionTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var timer = sender as Timer;
            timer.Enabled = false;

            SignalMessage(string.Format("Timer elapsed"));

            BusinessStateLogic.Set();
        }

        public event Delegates.OnSignalMessage SignalMessageEvent;

        protected void SignalMessage(string messageTemplate)
        {
            if (SignalMessageEvent != null)
                SignalMessageEvent.Invoke(messageTemplate);
        }

        public void Start()
        {
            Task.Factory.StartNew(RunningMachine, _cancellation.Token);
        }

        public void Stop()
        {
            if (_cancellation != null)
            {
                SignalMessage(string.Format("Cancellation requested, : {0}", DateTime.Now));
                _cancellation.Cancel();
            }
        }

        private void RunningMachine()
        {
            States currenState = States.StartUp;

            while (currenState != States.Stopped)
            {
                SignalMessage(string.Format("Current state {0}", currenState));
                if (_cancellation.IsCancellationRequested)
                {
                    ExecuteState(currenState);
                    currenState = CalculateNextState(currenState);
                    continue;
                }

                Task<bool> task = Task.Factory.StartNew(() => ExecuteState(currenState), _cancellation.Token);
                
                try
                {
                    task.Wait();

                    if (!task.Result)
                        currenState = States.Error;
                    else
                        currenState = CalculateNextState(currenState);
                }
                catch (AggregateException aEx)
                {
                    SignalMessage(string.Format("Task was cancelled : {0} {1}", aEx.Message, DateTime.Now));
                    currenState = CalculateNextState(currenState);
                }

                GC.Collect();
            }
        }

        private States CalculateNextState(States currentState)
        {
            if (_cancellation.IsCancellationRequested)
            {
                return currentState == States.Teardown
                           ? States.Stopped
                           : States.Teardown;
            }

            switch (currentState)
            {
                case States.StartUp:
                    return States.Initializing;

                case States.Initializing:
                    return States.Executing;

                case States.Executing:
                    return States.Executing;

                case States.Teardown:
                    return States.Stopped;

                case States.Error:
                    return States.Initializing;

                case States.Stopped:
                default:
                    return States.Stopped;
            }
        }

        private bool ExecuteState(States currenState)
        {
            switch (currenState)
            {
                case States.StartUp:
                    SignalMessage("State execution : StartUp");
                    return true;

                case States.Initializing:
                    SignalMessage("State execution : Initializing");

                    BusinessStateLogic.Initialize();
                    var timerLogic = BusinessStateLogic as ITimer;
                    if (timerLogic != null)
                    {
                        _timedExecutionTimer = timerLogic.GetTimer();
                        _timedExecutionTimer.Elapsed += _timedExecutionTimer_Elapsed;
                    }

                    return true;

                case States.Executing:
                    SignalMessage("State execution : Executing");

                    _timedExecutionTimer.Start();

                    BusinessStateLogic.ExecuteAfterTriggeredEvent();

                    return true;

                case States.Teardown:
                    SignalMessage("State execution : Teardown");
                    BusinessStateLogic.TearDown();
                    return true;

                case States.Error:
                    SignalMessage("State execution : Error");
                    return true;

                case States.Stopped:
                    // will never be called..
                    SignalMessage("State execution : Stopped");
                    return true;
                default:
                    throw new ArgumentOutOfRangeException("currenState");
            }
        }

        #region Nested type: States

        private enum States
        {
            StartUp = 0,
            Initializing = 1,
            Executing = 3,
            Teardown = 4,
            Error = 10,
            Stopped = 100
        }

        #endregion
    }
}
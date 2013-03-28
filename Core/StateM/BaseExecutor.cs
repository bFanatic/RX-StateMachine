using System;
using System.Threading;
using Timer = System.Timers.Timer;

namespace Core.StateM
{
    public interface ITimer
    {
        System.Timers.Timer GetTimer();
    }

    public abstract class BaseExecutor : EventWaitHandle
    {
        protected BaseExecutor()
            : base(false, EventResetMode.AutoReset)
        {
        }

        public event Delegates.OnSignalMessage SignalMessageEvent;

        protected void SignalMessage(string messageTemplate)
        {
            if (SignalMessageEvent != null)
                SignalMessageEvent.Invoke(messageTemplate);
        }


        public bool ExecuteAfterTriggeredEvent()
        {
            WaitOne();

            bool success = ExecuteLogic();

            return success;
        }

        protected abstract bool ExecuteLogic();
        public abstract void Initialize();
        public abstract void TearDown();
    }

    public class ConcreteExecutor : BaseExecutor, ITimer
    {
        private Timer _reocurrantTaskTimer;

        protected override bool ExecuteLogic()
        {
            SignalMessage("ConcreteExecutor - ExecuteLogic");
            Thread.Sleep(200);

            SignalMessage("ConcreteExecutor - ExecuteLogic -- some long action");
            Thread.Sleep(1500);

            SignalMessage("ConcreteExecutor - ExecuteLogic -- some other long action");
            Thread.Sleep(1500);

            SignalMessage("ConcreteExecutor - ExecuteLogic -- some short action");
            Thread.Sleep(200);

            return true;
        }

        public override void Initialize()
        {

            Thread.Sleep(200);
            SignalMessage("State execution : Initializing -- doing stuff 1");
            Thread.Sleep(200);
            SignalMessage("State execution : Initializing -- doing stuff 2");
            Thread.Sleep(200);
            SignalMessage("State execution : Initializing -- doing stuff 3");
            Thread.Sleep(200);
            SignalMessage("State execution : Initializing -- doing stuff 4");

            _reocurrantTaskTimer = new Timer {Interval = 3000};
        }

        public override void TearDown()
        {
            Thread.Sleep(200);
            SignalMessage("State execution : TearDown -- doing stuff AA");
            Thread.Sleep(1000);
            SignalMessage("State execution : TearDown -- doing stuff BB");
            Thread.Sleep(200);
            SignalMessage("State execution : TearDown -- doing stuff CC");
            Thread.Sleep(200);
            SignalMessage("State execution : TearDown -- doing stuff DD");
        }

        public Timer GetTimer()
        {
            return _reocurrantTaskTimer;
        }
    }
}
using System;
using Core.Queue;
using Core.StateM;

namespace ConsoleApp
{
    internal class Program
    {
        //private static Timer Guard;
        private static void Main(string[] args)
        {
            var machine = new BasicMachine();
            machine.SignalMessageEvent += tMachine_Trace;

            machine.Start();

            Console.ReadLine();
            machine.Stop();

            Console.ReadKey();
        }


        private void RX_Machine()
        {
            var tMachine = new TestMachine();
            tMachine.StateChanged += tMachine_StateChanged;
            tMachine.Trace += tMachine_Trace;

            tMachine.Start();

            Console.ReadKey();
            tMachine.Stop();

            Console.ReadKey();
        }

        private void OtherImplementation()
        {
            //        IObservable<long> ob = Observable.Create<long>(o =>
            //        {
            //            //var cancel = new CancellationDisposable();
            //            //// internally creates a new CancellationTokenSource

            //            //var task = ScheduledTask(cancel, o);

            //            //Guard = new Timer(x => o.OnCompleted(), null, 5000, 5000);

            //            ////NewThreadScheduler.Default.Schedule(ScheduledTask(cancel, o));
            //            //NewThreadScheduler.Default.Schedule(task);

            //            return cancel;
            //        }
            //);

            //        IDisposable subscription = ob.Subscribe(Console.WriteLine);
            //        Console.WriteLine("Press any key to cancel");
            //        Console.ReadKey();
            //        subscription.Dispose();
            //        Console.WriteLine("Press any key to quit");
            //        Console.ReadKey(); // give background thread chance to write the cancel acknowledge message
        }

        private static void tMachine_Trace(string message)
        {
            Console.WriteLine("Message      : " + message);
        }

        private static void tMachine_StateChanged(QueueStates state)
        {
            Console.WriteLine("                                          State Change : " + state);
        }

        //private void WatchGuard(IObserver<long> observer)
        //{
        //    observer.OnCompleted();
        //}

        //private static Action ScheduledTask(CancellationDisposable cancel, IObserver<long> o)
        //{
        //    return () =>
        //               {
        //                   long i = 0;
        //                   long ii = 1;
        //                   for (; ; )
        //                   {
        //                       Thread.Sleep(100); // here we do the long lasting background operation
        //                       if (!cancel.Token.IsCancellationRequested) // check cancel token periodically
        //                       {
        //                           long iii = i + ii;
        //                           i = ii;
        //                           ii = iii;
        //                           o.OnNext(iii);
        //                       }
        //                       else
        //                       {
        //                           Console.WriteLine("Aborting because cancel event was signaled!");
        //                           o.OnCompleted();
        //                           return;
        //                       }
        //                   }
        //               };
        //}
    }
}
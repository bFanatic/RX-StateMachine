using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;

namespace Core.StateMachine
{
    public class RxWithCancelation
    {
        public IObservable<int> ObservableCollection = Observable.Create<int>(o => ScheduledMethod(o));

        private static IDisposable ScheduledMethod(IObserver<int> o)
        {
            var cancel = new CancellationDisposable();
            // internally creates a new CancellationTokenSource
            NewThreadScheduler.Default.Schedule(() =>
                                                    {
                                                        int i = 0;
                                                        for (;;)
                                                        {
                                                            Thread.Sleep(200);
                                                            // here we do the long lasting background operation
                                                            if (!cancel.Token.IsCancellationRequested)
                                                                // check cancel token periodically
                                                                o.OnNext(i++);
                                                            else
                                                            {
                                                                Console.WriteLine(
                                                                    "Aborting because cancel event was signaled!");
                                                                o.OnCompleted();
                                                                return;
                                                            }
                                                        }
                                                    }
                );

            return cancel;
        }
    }
}
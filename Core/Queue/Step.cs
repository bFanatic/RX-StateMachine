using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;

namespace Core.Queue
{
    public abstract class BaseStep
    {
        public CancellationDisposable Cancellation { private get; set; }

        public abstract QueueStates CurrentStep { get; }
        public abstract QueueStates NextStep { get; }

        public abstract bool Execute();

    }
}

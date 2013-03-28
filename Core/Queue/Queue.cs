using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using System.Text;

namespace Core.Queue
{
    public abstract class Queue
    {
        // four states
        // init
        // wait for execution
        // execute after triggered
        // teardown and goto wait for execution or init


        protected virtual void Initialize(IObserver<QueueStates> state)
        {

        }
        protected virtual void Execute()
        {
        }
        protected virtual void Teardown()
        {
        }

    }
}


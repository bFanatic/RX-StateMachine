using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Queue
{
    public enum QueueStates
    {
        Initializing = 1,
        WaitingForTrigger = 2,
        Executing = 3,
        Teardown = 4,
        Error = 10,
        StartUp = 0,
        Stopped = 100
    }
}

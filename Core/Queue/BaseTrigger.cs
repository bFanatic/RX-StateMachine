using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Queue
{
    public abstract class BaseTrigger
    {
        public abstract string Name { get; }
    }

    public class TimerTrigger : BaseTrigger
    {
        public override string Name
        {
            get { return "Timer"; }
        }
    }
}

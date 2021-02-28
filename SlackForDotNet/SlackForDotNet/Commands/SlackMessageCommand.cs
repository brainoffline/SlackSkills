using System;
using System.Collections.Generic;

using Asciis;

namespace SlackForDotNet
{
    public abstract class SlackMessageCommand
    {
        public          ISlackApp? SlackApp { get; set; }
        public          Message?   Message  { get; set; }
        
        public abstract void       OnCommand(List<string> extras);

        [Action("Execute", "Default action", DefaultAction = true)]
        public void Execute(List<string> extras)
        {
            OnCommand(extras);
        }
    }

}

using System;
using System.Collections.Generic;

using Asciis;

namespace SlackSkills
{
    public abstract class SlackGlobalShortcutCommand
    {
        public ISlackApp?               SlackApp { get; set; }
        public GlobalShortcut?          Shortcut { get; set; }

        public abstract void OnCommand(List<string> args);

        [Action( "Execute", "Default action", DefaultAction = true)]
        public void Execute(List<string> args)
        {
            OnCommand(args);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Asciis;

namespace SlackSkills
{
    public abstract class SlackMessageShortcutCommand
    {
        public ISlackApp?       SlackApp { get; set; }
        public MessageShortcut? Shortcut { get; set; }

        public abstract void OnCommand(List<string> args);

        [Action( "Execute", "Default action", DefaultAction = true)]
        public void Execute(List<string> args)
        {
            OnCommand(args);
        }

    }
}

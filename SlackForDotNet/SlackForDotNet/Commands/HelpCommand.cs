using System;
using System.Collections.Generic;

using Asciis;

namespace SlackForDotNet
{
    [Command( "help|?", Description = "Display a list of all commands")]
    public class HelpCommand : BaseSlackCommand
    {
        public override void OnCommand(List<string> extras)
        {
            // Replay to message
            SlackApp?.Say("Available Commands\n\n```" + SlackApp.CommandHelp() + "```",
                         Message?.channel ?? "",
                         //Message?.user,   // don't know why the message is swallowed by slack with user id present
                         ts: Message?.ts);
        }
    }
}

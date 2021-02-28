using System;
using System.Collections.Generic;

using Asciis;

using SlackForDotNet;

namespace SampleCLI
{
    [Command( "Blah", Description = "Blah command using Slack as a command line interpreter")]
    internal class BlahMessageCommand : SlackMessageCommand
    {
        [Param( "b|Blah", "Blah parameter")]
        public string? Blah { get; set; }

        public override void OnCommand(List<string> extras)
        {
            if (extras.Count > 0)
                Blah = extras[0];
            SlackApp?.Say(string.IsNullOrWhiteSpace(Blah)
                                       ? "Hello"
                                       : $"Hello to {Blah}. :wave:",
                                   Message!.channel);
        }
    }
}

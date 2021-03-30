using System;
using System.Collections.Generic;

using Asciis;

using SlackSkills;
using SlackSkills.Surface;
using SlackSkills.WebApiContracts;

namespace SampleCLI
{
    [Command("slackdk_action", Description = "Example Message Shortcut")]
    public class ExampleMessageShortcutCommand : SlackMessageShortcutCommand
    {
        public override void OnCommand(List<string> args)
        {
            var request = new ViewOpen
                          {
                              trigger_id = Shortcut!.trigger_id,
                              view = new ModalView
                                     {
                                         title = Shortcut.callback_id!,
                                         close = "Thanks",
                                     }
                                 .Add(new SectionLayout { block_id = "wave_id", text = ":wave: Nice message" })
                          };
            var response = SlackApp!.Send<ViewOpen, ViewResponse>(request);
        }
    }
}

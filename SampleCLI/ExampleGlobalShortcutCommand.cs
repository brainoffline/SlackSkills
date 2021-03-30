using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Asciis;

using SlackSkills;
using SlackSkills.Surface;
using SlackSkills.WebApiContracts;

namespace SampleCLI
{
    [Command("slackdk_shortcut", Description = "Example Global Shortcut")]
    public class ExampleGlobalShortcutCommand : SlackGlobalShortcutCommand
    {
        public override void OnCommand( List< string > args )
        {
            var request = new ViewOpen
                          {
                              trigger_id = Shortcut!.trigger_id,
                              view = new ModalView
                                     {
                                         title  = Shortcut.callback_id!,
                                         close  = "Thanks",
                                         submit = "Do Something"
                                     }
                                    .Add(new SectionLayout { block_id = "wave_id", text = ":wave: Aren't shortcuts awesome" })
                                    .Add(new SectionLayout
                                         {
                                             block_id = "button-section",
                                             text     = "*Welcome*",
                                             accessory = new ButtonElement
                                                         {
                                                             text      = "Just a button",
                                                             action_id = "just-a-button"
                                                         }
                                         })
                          };
            var response = SlackApp!.Send<ViewOpen, ViewResponse>(request);
        }
    }
}

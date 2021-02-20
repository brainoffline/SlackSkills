using System;
using System.Threading.Tasks;

using Asciis;

using Microsoft.Extensions.Logging;

using SlackForDotNet;
using SlackForDotNet.Surface;
using SlackForDotNet.WebApiContracts;

namespace SampleCLI
{
    public partial class Program
    {
        private HametabSurface? _home;
        
        public void SetupSurface()
        {
            _app.OnMessage<AppMention>(OnAppMention);

            _app.OnSlashCommand("/slackdk", OnSlashCommandSurface);

            _app.OnGlobalShortcut("slackdk_shortcut", OnShortcutSurface);
            _app.OnMessageShortcut("slackdk_action", OnMessageShortcutSurface);

            _app.OnMessage<AppHomeOpened>(OnHomeOpenedSurface);

        }

        async void OnAppMention(ISlackApp slackApp, AppMention msg)
        {
            await slackApp.SayToUser("Thanks for the mention brah", null, msg.channel, msg.user);
        }

        void OnSlashCommandSurface(ISlackApp slackApp, SlashCommand msg, IEnvelope<SlashCommand> envelope)
        {
            var parser  = new ParamParser<SlashCommandExample>();
            var example = parser.ParseArguments(msg.text, includeEnvironmentVariables: false);

            if (string.IsNullOrWhiteSpace(example.Comment))
            {
                var response = new SlashCommandResponse
                               {
                                   response_type = "ephemeral", // visible to only the user
                                   text = parser.Help()
                               };
                slackApp.Push( new AcknowledgeResponse< SlashCommandResponse >
                               ( envelope.envelope_id, response ));
            }
            else
            {
                var response = new SlashCommandResponse()
                              .Add( new HeaderLayout( "Slash command response" ) )
                              .Add( new SectionLayout { block_id = "wave_id", text = $":wave: {example.Comment}" } )
                              .Add( new SectionLayout
                                    {
                                        block_id = "button-section", 
                                        text = "*Welcome*", 
                                        accessory = new ButtonElement { text = "Just a button", action_id = "just-a-button" }
                                    } );
                slackApp.Push( new AcknowledgeResponse< SlashCommandResponse >(
                       envelope.envelope_id, response ) );
            }
        }

        private void OnShortcutSurface(ISlackApp slackApp, GlobalShortcut msg)
        {
            var request = new ViewOpen
                          {
                              trigger_id = msg.trigger_id,
                              view = new ModalView
                                     {
                                         title           = msg.callback_id!,
                                         close           = "Thanks",
                                         submit          = "Do Something"
                                     }
                                    .Add(new SectionLayout { block_id = "wave_id", text = ":wave: Aren't shortcuts awesome" })
                                    .Add(new SectionLayout
                                         {
                                             block_id  = "button-section",
                                             text      = "*Welcome*",
                                             accessory = new ButtonElement
                                                         {
                                                             text = "Just a button", 
                                                             action_id = "just-a-button"
                                                         }
                                         })
                          };
            var response = slackApp.Send< ViewOpen, ViewResponse >( request );
        }

        private void OnMessageShortcutSurface(ISlackApp slackApp, MessageShortcut msg)
        {
            var request = new ViewOpen
                          {
                              trigger_id = msg.trigger_id,
                              view = new ModalView
                                     {
                                         title  = msg.callback_id!,
                                         close  = "Thanks",
                                     }
                                    .Add(new SectionLayout { block_id = "wave_id", text = ":wave: Nice message" })
                          };
            var response = slackApp.Send<ViewOpen, ViewResponse>(request);
        }

        private async void OnHomeOpenedSurface(ISlackApp slackApp, AppHomeOpened msg)
        {
            _home ??= new HametabSurface(_app);

            await _app.PublishHomepage(_home, msg);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private void SetupRaw()
        {
            Console.WriteLine("Slack CLI");

            // Register any messages, commands, events, actions or shortcuts

            _app.OnMessage<AppHomeOpened>(OnHomeOpened);
            _app.OnMessage<AppMention>(OnAppMention);
            _app.OnMessage<SlashCommands>(OnSlashCommand);
            _app.OnMessage<Shortcut>(OnShortcut);
            _app.OnMessage<ViewSubmission>(OnViewSubmission);
            _app.OnMessage<BlockActions>(OnBlockActions);
            _app.OnMessage<ViewClosed>(OnViewClosed);

            // Parsing whatever is typed into Slack
            _app.RegisterCommand<ExampleCommand>();
        }

        // Message Handlers
        // Select the App from the SlideBar
        async void OnHomeOpened(ISlackApp slackApp, AppHomeOpened msg)
        {
            slackApp.Logger.LogInformation("Home Opened");
            if (msg.view == null)
            {
                var request = new ViewsPublish
                {
                    user_id = msg.user,
                    view = new HometabView { callback_id = "home-callback" }
                                        .Add(new SectionLayout { text = ":wave: hello" })
                                        .Add(new SectionLayout
                                        {
                                            text = "It's a kind of",
                                            block_id = "magic-block",
                                            accessory = new ButtonElement { action_id = "magic-button", text = "Magic" }
                                        })
                };

                var response = await slackApp.Send<ViewsPublish, ViewsPublishResponse>(request);
                if (response?.ok == true)
                {
                    slackApp.Logger.LogInformation("Welcome home");
                }
            }
        }

        async void OnAppMention(ISlackApp slackApp, AppMention msg)
        {
            slackApp.Logger.LogDebug("App Mention");

            await slackApp.SayToUser("Thanks for the mention brah", null, msg.channel, msg.user);
        }


        void OnSlashCommand(ISlackApp slackApp, SlashCommands msg)
        {
            if (msg.payload == null)
                return;
            slackApp.Logger.LogInformation($"Slash {msg.payload.command} {msg.payload.text}");

            var parser  = new ParamParser<SlashExample>();
            var example = parser.ParseArguments(msg.payload.text, includeEnvironmentVariables: false);

            if (string.IsNullOrWhiteSpace(example.ServerName))
            {
                // Include the user id so that only that user will see the message
                slackApp.Say(parser.Help(), msg.payload.channel_id, msg.payload.user_id); // Ephemeral msg, just to user 
            }
            else
            {
                slackApp.Say(new SectionLayout
                {
                    text = "I have my :eyes: on you\nWhat do you think you are up to :question:"
                },
                              msg.payload.channel_id);
            }
        }

        async void OnShortcut(ISlackApp slackApp, Shortcut msg)
        {
            var request = new ViewOpen
            {
                trigger_id = msg.trigger_id,
                view = new ModalView { title = "And here we are", close = "Thanks", submit = "Do Something", notify_on_close = true }
                                    .Add(new SectionLayout { block_id = "wave_id", text = ":wave: Aren't shortcuts awesome" })
                                    .Add(new SectionLayout
                                    {
                                        block_id = "button-section",
                                        text = "*Welcome*",
                                        accessory = new ButtonElement { text = "Just a button", action_id = "just-a-button" }
                                    })
            };

            var response = await slackApp.Send<ViewOpen, MessageResponse>(request);
            if (response?.ok == true)
            {
                slackApp.Logger.LogInformation("Shortcut message");
            }
        }

        void OnViewSubmission(ISlackApp slackApp, ViewSubmission msg)
        {
            slackApp.Logger.LogDebug("View Submission");
        }

        async void OnBlockActions(ISlackApp slackApp, BlockActions msg)
        {
            slackApp.Logger.LogDebug("Block Actions");

            if (msg.actions == null)
                return;

            if (msg.actions.Any(a => a.action_id == "magic-button"))
            {
                var request = new ViewsPublish
                {
                    user_id = msg.user.id,
                    view = new HometabView { callback_id = "home-callback" }
                                        .Add(new SectionLayout { text = ":sob: magic is hard" })
                };
                var response = await slackApp.Send<ViewsPublish, ViewsPublishResponse>(request);
                if (response?.ok == true)
                {
                    slackApp.Logger.LogInformation("home updated");
                }
            }
        }

        void OnViewClosed(ISlackApp slackApp, ViewClosed msg)
        {
            slackApp.Logger.LogDebug("View Closed");
        }
    }
}

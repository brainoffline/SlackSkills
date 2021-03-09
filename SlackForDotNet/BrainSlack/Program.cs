using SlackForDotNet;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Asciis;

using BrainSlack.Helpers;

using Microsoft.Extensions.Logging;

using SlackForDotNet.Surface;
using SlackForDotNet.WebApiContracts;

using static SlackForDotNet.SlackConstants;

namespace BrainSlack
{
    class Program
    {
        #pragma warning disable 8618
        private SlackApp    _app;
        private CommandLine _config;
        #pragma warning restore 8618

        static async Task Main(string[] args)
        {
            var program = new Program();

            program.SetupSlackApp(args);
            program.RegisterMessageTypes();

            await program.Run();
        }

        private void SetupSlackApp(string[] args)
        {
            // Parse any command line arguments and environment variables
            _config = ParamParser<CommandLine>.Parse(args);

            // Setup Slack integration
            // Depending on the scenario, you will need to supply only some fields
            //  - SocketMode app.          AppLevelToken from https://api.slack.com/apps/{appId}/general
            //  - Make calls as a Bot.     BotAccessToken or (ClientId + ClientSecret + BotScopes + RedirectUrl for browser login)
            //  - Make calls as a User.    UserAccessToken or ((ClientId + ClientSecret + UserScopes + RedirectUrl for browser login)
            // RedirectUrl is optional for browser login. The default url will be http://localhost:3100/ if it is not supplied
            //   This url must match the Redirect URLs in your app settings: https://api.slack.com/apps/{appId}/oauth
            _app = new SlackApp
            {
                ClientId = _config.ClientId,
                ClientSecret = _config.ClientSecret,
                AppLevelToken = _config.AppLevelToken,
                BotAccessToken = _config.BotAccessToken,
                BotScopes = _config.BotScopes ?? DefaultBotScope,
                UserAccessToken = _config.UserAccessToken,
                UserScopes = _config.UserScopes ?? DefaultUserScope,
                RedirectUrl = _config.RedirectUrl ?? DefaultRedirectUrl,
                AccessTokensUpdated = slackApp =>
                {
                    // If you aren't supplying a bot or user access token, 
                    // After the user logs in using the browser
                    Console.WriteLine($"Bot Access Token: {slackApp.BotAccessToken}");
                    Console.WriteLine($"User Access Token: {slackApp.UserAccessToken}");
                    // TODO: Save new access tokens to a safe place
                },
                LogBuilder = builder =>
                {
                    // Microsoft Logging framework. Configure as you feel best
                    // TODO: configure the Logger builder
                    builder
#if DEBUG
                                          .SetMinimumLevel(LogLevel.Debug)
#endif
                                          .AddDebug()
                       .AddConsole(options =>
                       {
                           options.LogToStandardErrorThreshold = LogLevel.Debug;
                       });
                }
            };
        }

        private async Task Run()
        {
            // Setup is done, now
            // Connect to slack
            var success = await _app.Connect();
            if (!success)
            {
                var help = new ParamParser<CommandLine>().Help();
                Console.WriteLine(help);

                return;
            }

            // Hold the console open
            do
            {
                Console.WriteLine("Enter 'Stop' to exit");
                var line = Console.ReadLine();

                if (string.Compare(line, "Stop", StringComparison.OrdinalIgnoreCase) == 0)
                    break;

                if (string.Compare(line, "cls", StringComparison.OrdinalIgnoreCase) == 0)
                    Console.Clear();

            } while (true);

            Console.WriteLine("Slack CLI Stopped");
        }

        public void RegisterMessageTypes()
        {
            _app.RegisterSlashCommand<EstimateSlashCommand>();
            _app.OnMessage< Message >( OnMessage );
        }

        private async void OnMessage( ISlackApp slackApp, Message msg )
        {
            if (msg.text != null)
            {
                if (AnnoyingGreeting( msg.text ))
                {
                    var context = await slackApp.GetUserContext( msg.user );
                    var name    = context?.User.real_name ?? context?.User.name ?? "";
                    _ = slackApp.AddReaction(msg, "see_no_evil");
                    _ = slackApp.Say( $"http://www.nohello.com Please visit this site, {name}.", 
                                     msg.channel, user: msg.user, ts: msg.thread_ts );
                }

                if (BadWords.ContainsBadWords( msg.text ))
                {
                    var context = await slackApp.GetUserContext(msg.user);
                    _ = slackApp.AddReaction(msg, "face_with_symbols_on_mouth");
                }
            }
        }

        private static bool AnnoyingGreeting(string text)
        {
            return ( text.IgnoreCaseEquals( "hi" )
                  || text.IgnoreCaseEquals( "hello" )
                  || text.IgnoreCaseEquals( "gidday" ) );
        }

        // Open up you https://api.slack.com/apps/  Event subscriptions. Update to include *ONLY* the scopes you need
        private static readonly string DefaultBotScope = BuildScope(
            BotScopes.Commands,
            BotScopes.Channels_History,
            BotScopes.Chat_Write,
            BotScopes.Reactions_Write, 
            BotScopes.Users_Read,
            BotScopes.UsersProfile_Read);

        private static readonly string DefaultUserScope = BuildScope();
    }
}

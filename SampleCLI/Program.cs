﻿using Asciis;
using Microsoft.Extensions.Logging;
using SlackSkills;

using System;
using System.Threading.Tasks;

using static SlackSkills.SlackConstants;

// ReSharper disable CheckNamespace

namespace SampleCLI
{
    public class Program
    {
        #pragma warning disable 8618
        private SlackApp    _app;
        private CommandLine _config;
        #pragma warning restore 8618

        public static async Task Main( string[] args )
        {
            var program = new Program();

            program.SetupSlackApp( args );
            program.RegisterMessageTypes();

            await program.Run();
        }

        private void SetupSlackApp( string[] args )
        {
            // Parse any command line arguments and environment variables
            _config = ParamParser< CommandLine >.Parse( args );

            // Setup Slack integration
            // Depending on the scenario, you will need to supply only some fields
            //  - SocketMode app.          AppLevelToken from https://api.slack.com/apps/{id}/general
            //  - Make calls as a Bot.     BotAccessToken or (ClientId + ClientSecret + BotScopes + RedirectUrl for browser login)
            //  - Make calls as a User.    UserAccessToken or ((ClientId + ClientSecret + UserScopes + RedirectUrl for browser login)
            // RedirectUrl is optional for browser login. The default url will be http://localhost:3100/ if it is not supplied
            _app = new SlackApp
                  {
                      ClientId        = _config.ClientId,
                      ClientSecret    = _config.ClientSecret,
                      AppLevelToken   = _config.AppLevelToken,
                      BotAccessToken  = _config.BotAccessToken,
                      BotScopes       = _config.BotScopes ?? DefaultBotScope,
                      UserAccessToken = _config.UserAccessToken,
                      UserScopes      = _config.UserScopes  ?? DefaultUserScope,
                      RedirectUrl     = _config.RedirectUrl ?? DefaultRedirectUrl,
                      AccessTokensUpdated = slackApp =>
                                            {
                                                // If you aren't supplying a bot or user access token, 
                                                // After the user logs in using the browser
                                                Console.WriteLine( $"Bot Access Token: {slackApp.BotAccessToken}" );
                                                Console.WriteLine( $"User Access Token: {slackApp.UserAccessToken}" );
                                                // TODO: Save new access tokens to a safe place
                                            },
                      LogBuilder = builder =>
                                   {
                                       // Microsoft Logging framework. Configure as you feel best
                                       // TODO: configure the Logger builder
                                       builder
                                       #if DEBUG
                                          .SetMinimumLevel( LogLevel.Debug )
                                       #endif
                                          .AddDebug()
                                          .AddConsole( options =>
                                                       {
                                                           options.LogToStandardErrorThreshold = LogLevel.Debug;
                                                       } );
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
                var help = new ParamParser< CommandLine >().Help();
                Console.WriteLine( help );

                return;
            }

            // Hold the console open
            do
            {
                Console.WriteLine( "Enter 'Stop' to exit" );
                var line = Console.ReadLine();

                if (string.Compare( line, "Stop", StringComparison.OrdinalIgnoreCase ) == 0)
                    break;
                
                if (string.Compare( line, "cls", StringComparison.OrdinalIgnoreCase ) == 0)
                    Console.Clear();

            } while (true);

            Console.WriteLine( "Slack CLI Stopped" );
        }

        public void RegisterMessageTypes()
        {
            _app.OnMessage<AppMention>(OnAppMention);

            _app.RegisterHometabSurface<ExampleHometabSurface>();
            _app.RegisterGlobalShortcutCommand<ExampleGlobalShortcutCommand>();
            _app.RegisterMessageShortcutCommand<ExampleMessageShortcutCommand>();

            _app.RegisterSlashCommand<ExampleSlashCommand>();
            _app.RegisterMessageCommand<ExampleMessageCommand>();
            _app.RegisterMessageCommand<BlahMessageCommand>();
            _app.RegisterMessageCommand<YapMessageCommand>();
        }

        /// <summary>
        ///     Examples of different ways to send a message
        /// </summary>
        async void OnAppMention(ISlackApp slackApp, AppMention msg)
        {
            // Visible to everyone in the channel
            await slackApp.Say( "Thanks for the mention brah", msg.channel );

            // Visible to only the user in the channel
            await slackApp.Say("Thanks for the mention brah", msg.channel, msg.user);

            // Visible as a thread off the main channel
            await slackApp.Say( "Thanks for the mention brah", msg.channel, user:null, msg.ts );

            // Sent to the bot messages
            await slackApp.Say("Thanks for the mention brah", msg.user);
        }

        // Open up you https://api.slack.com/apps/  Event subscriptions. Update to include *ONLY* the scopes you need
        private static readonly string DefaultBotScope = BuildScope(
                                                                    BotScopes.AppMentions_Read,
                                                                    BotScopes.Chat_Write,
                                                                    BotScopes.Chat_Write_Public,
                                                                    BotScopes.Channels_Read,
                                                                    BotScopes.Channels_Join,
                                                                    BotScopes.Commands,
                                                                    BotScopes.DoNotDisturb_Read,
                                                                    BotScopes.Emoji_Read,
                                                                    BotScopes.Groups_Read,
                                                                    BotScopes.InstantMessage_History,
                                                                    BotScopes.InstantMessage_Write,
                                                                    BotScopes.Links_Read,
                                                                    BotScopes.Links_Write,
                                                                    BotScopes.Team_Read,
                                                                    BotScopes.Users_Read,
                                                                    BotScopes.UsersProfile_Read );

        private static readonly string DefaultUserScope = BuildScope(
                                                                     UserScopes.Channels_History,
                                                                     UserScopes.Channels_Read,
                                                                     UserScopes.Chat_Write,
                                                                     UserScopes.Emoji_Read,
                                                                     UserScopes.Groups_History,
                                                                     UserScopes.Groups_Read,
                                                                     UserScopes.InstantMessage_Read,
                                                                     UserScopes.Links_Read,
                                                                     UserScopes.MPIM_Read,
                                                                     UserScopes.Reminders_Read,
                                                                     UserScopes.Reminders_Write,
                                                                     UserScopes.Search_Read,
                                                                     UserScopes.Stars_Read,
                                                                     UserScopes.Team_Read,
                                                                     UserScopes.Users_Read
                                                                    );


    }

    [ Command( "SampleCLI", Description = "Sample Slack CLI using SlackSkills" ) ]
    class CommandLine
    {
        [ Param( "i|Id|ClientId", "Slack Client Id. Mandatory" ) ]
        public string? ClientId { get; set; }

        [ Param( "s|Secret|ClientSecret", "Slack Client Secret. Mandatory" ) ]
        public string? ClientSecret { get; set; }

        [ Param( "a|AppLevel|AppLevelToken", "App Level Token. Required for SocketMode" ) ]
        public string? AppLevelToken { get; set; }

        [ Param( "b|Bot|BotAccessToken", "Bot access token. If not supplied, browser will be launched to login" ) ]
        public string? BotAccessToken { get; set; }

        [ Param( "u|User|UserAccessToken", "User access token. If user scopes are supplied and no access token, browser will be launched to login" ) ]
        public string? UserAccessToken { get; set; }

        [ Param( "bs|BotScopes", "Bot Scopes (comma separated). Required if no Bot access token supplied" ) ]
        public string? BotScopes { get; set; }

        [ Param( "us|UserScopes", "User Scopes (comma separated). Indicates a user access token is required" ) ]
        public string? UserScopes { get; set; }

        [ Param( "r|Redirect|RedirectUrl", "Url used to host browser calls during login (OAuth 2.0 security)" ) ]
        public string? RedirectUrl { get; set; }
    }
}

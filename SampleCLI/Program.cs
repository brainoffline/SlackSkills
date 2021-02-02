using Asciis;
using Microsoft.Extensions.Logging;
using SlackForDotNet;
using SlackForDotNet.WebApiContracts;
using System;
using System.Collections.Generic;

using static SlackForDotNet.SlackConstants;

// ReSharper disable CheckNamespace


Console.WriteLine( "Slack CLI" );

// Parse any command line arguments and environment variables
var config = ParamParser<CommandLine>.Parse( args );

// Open up you https://api.slack.com/apps/  Event subscriptions. Update to include ONLY the scopes you need
var defaultBotScope  = BuildScope( 
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
                                  BotScopes.UsersProfile_Read);
var defaultUserScope = BuildScope( 
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

// Setup Slack integration
// Depending on the scenario, you will need to supply the particular fields
//  - SocketMode app.          AppLevelToken from https://api.slack.com/apps/{id}/general
//  - Make calls as a Bot.     BotAccessToken or (ClientId + ClientSecret + BotScopes + RedirectUrl for browser login)
//  - Make calls as a User.    UserAccessToken or ((ClientId + ClientSecret + UserScopes + RedirectUrl for browser login)
// RedirectUrl is optional for browser login. The default url will be http://localhost:3100/ if it is not supplied
var app = new SlackApp
          {
              ClientId        = config.ClientId,
              ClientSecret    = config.ClientSecret,
              AppLevelToken   = config.AppLevelToken,
              BotAccessToken  = config.BotAccessToken,
              BotScopes       = config.BotScopes ?? defaultBotScope,
              UserAccessToken = config.UserAccessToken,
              UserScopes      = config.UserScopes  ?? defaultUserScope,
              RedirectUrl     = config.RedirectUrl ?? DefaultRedirectUrl,
              AccessTokensUpdated = slackApp =>
                                    {
                                        Console.WriteLine( $"Bot Access Token: {slackApp.BotAccessToken}" );
                                        Console.WriteLine( $"User Access Token: {slackApp.UserAccessToken}" );
                                        // TODO: Save new access tokens to a safe place
                                    },
              LogBuilder = builder =>
                           {
                               // TODO: configure the Logger builder
                               builder
#if DEBUG
                                  .SetMinimumLevel(LogLevel.Debug)
#endif
                                  .AddDebug()
                                  .AddConsole( options =>
                                               {
                                                   options.LogToStandardErrorThreshold = LogLevel.Debug;
                                               } );
                           }
          };

// Register any messages, commands, events, actions or shortcuts

app.OnMessage< AppHomeOpened >( OnHomeOpened );
app.OnMessage< SlashCommands >( OnSlashCommand );
app.OnMessage< Shortcut >( OnShortcut );

app.RegisterCommand<ExampleCommand>();


// Connect to slack
var success = await app.Connect();
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

} while (true);

Console.WriteLine("Slack CLI Stopped");



// Message Handlers

async void OnHomeOpened( ISlackApp slackApp, AppHomeOpened msg )
{
    slackApp.Logger.LogInformation("Home Opened");
    if (msg.view == null)
    {
        var request = new ViewsPublish
                      {
                          user_id = msg.user,
                          view = new HometabView
                                 {
                                     callback_id = "home-callback"
                                 }
                                 .Add( new SectionBlock { text = ":wave: hello" } )
                      };

        var response = await app.Send< ViewsPublish, MessageResponse >( request );
        if (response?.ok == true)
        {
            app.Logger.LogInformation("Welcome home");
        }
    }
}

void OnSlashCommand( ISlackApp slackApp, SlashCommands msg )
{
    slackApp.Logger.LogInformation($"Slash {msg.payload.command} {msg.payload.text}");

    var parser  = new ParamParser< SlashExample >();
    var example = parser.ParseArguments( msg.payload.text, includeEnvironmentVariables: false );

    if (string.IsNullOrWhiteSpace( example.ServerName ))
    {
        // Include the user id so that only that user will see the message
        slackApp.Say( parser.Help(), msg.payload.channel_id, msg.payload.user_id ); // Ephemeral msg, just to user 
    }
    else
    {
        slackApp.Say( new SectionBlock
                      {
                          text = "I have my :eyes: on you\nWhat do you think you are up to :question:"
        }, 
                     msg.payload.channel_id );
    }
}

async void OnShortcut( ISlackApp slackApp, Shortcut msg )
{
    var request = new ViewOpen
                  {
                      trigger_id = msg.payload.trigger_id,
                      view = new ModalView
                             {
                                 title = "And here we are",
                                 close = "Thanks",
                                 submit = "Do Something",
                                 notify_on_close = true
                             }.Add(new SectionBlock
                                   {
                                       block_id = "wave_id",
                                       text = ":wave: Aren't shortcuts awesome"
                                   })
                  };

    var response = await app.Send<ViewOpen, MessageResponse>(request);
    if (response?.ok == true)
    {
        app.Logger.LogInformation("Shortcut message");
    }
}


internal class SlashExample
{
    [Param("s|Server", "Server Name")]
    public string? ServerName { get; set; }
}


[Command( "ExampleCommand", Description = "Example of using Slack as a command line interpreter")]
internal class ExampleCommand : BaseSlackCommand
{
    [Param( "n|Name", "Name")]
    public string? Name { get; set; }

    public override void OnCommand( List< string > extras )
    {
        if (extras.Count > 0)
            Name = extras[ 0 ];
        SlackApp?.SayToChannel( string.IsNullOrWhiteSpace( Name )
                                    ? "Example Hello"
                                    : $"Example Hello to {Name}. :wave:",
                               null,
                               channel: Message?.channel ?? "" );
        
        Console.WriteLine("Example Command Executed");
    }
}




[Command( "SampleCLI", Description = "Sample Slack CLI using SlackForDotNet" ) ]
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

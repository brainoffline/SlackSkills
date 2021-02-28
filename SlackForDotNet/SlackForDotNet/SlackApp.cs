using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Asciis;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;

using SlackForDotNet.Surface;
using Newtonsoft.Json;

using SlackForDotNet.WebApiContracts;

namespace SlackForDotNet
{
    /// <summary>
    ///     Slack integration app
    /// </summary>
    /// <remarks>
    /// Depending on the scenario, you will need to supply the particular fields
    ///  - SocketMode app.          AppLevelToken from https://api.slack.com/apps/{id}/general
    ///  - Make calls as a Bot.     BotAccessToken or (ClientId + ClientSecret + BotScopes + RedirectUrl for browser login)
    ///  - Make calls as a User.    UserAccessToken or ((ClientId + ClientSecret + UserScopes + RedirectUrl for browser login)
    ///
    /// RedirectUrl is optional for browser login. The default url will be http://localhost:3100/ if it is not supplied
    /// </remarks>
    public class SlackApp : ISlackApp
    {
        public string?                  ClientId            { get; set; }
        public string?                  ClientSecret        { get; set; }
        public string?                  AppLevelToken       { get; set; }
        public string?                  BotAccessToken      { get; set; }
        public string?                  UserAccessToken     { get; set; }
        public string?                  BotScopes           { get; set; }
        public string?                  UserScopes          { get; set; }
        public string?                  RedirectUrl         { get; set; }
        public Action<SlackApp>?        AccessTokensUpdated { get; set; }
        public Action<ILoggingBuilder>? LogBuilder          { get; set; }
        
        public ILogger<SlackApp>? Logger { get; set; }
        public string             AppId  { get; set; } = "";
        public string             TeamId { get; set; } = "";

        private          bool                  _socketModeAvailable;
        private          bool                  _getBotAccessToken;
        private          bool                  _getUserAccessToken;
        private          SlackClient?          _slackClient;
        private          SlackSocket?          _slackSocket;
        private          OAuthAccessResponse2? _oauth;
        private readonly List< User >          _users       = new();
        private readonly List< Channel >       _channels    = new();
        private readonly List< UserGroup >?    _userGroups  = new();
        private          SlackSurface?         _hometabView = null;
        private          Type?                 _hometabType;


        private readonly List< ApiEventHandler >            _apiEventHandlers    = new();
        private readonly List< CommandHandler >             _commandHandlers     = new();
        private readonly List< IParamParser >               _paramParsers        = new();
        private readonly List< SlackSurface >               _surfaces            = new();

        static SlackApp()
        {
            MessageTypes.RegisterAll();
            JsonConvert.DefaultSettings = () =>
                                              new JsonSerializerSettings
                                              {
                                                  Formatting        = Formatting.Indented,
                                                  NullValueHandling = NullValueHandling.Ignore
                                              };
        }
        
        bool IsValid()
        {
            if (string.IsNullOrWhiteSpace( AppLevelToken ))
            {
                Logger?.LogDebug( "SocketMode not available. Required App-Level token" );
                _socketModeAvailable = false;
            }
            else
                _socketModeAvailable = true;

            if (string.IsNullOrWhiteSpace(BotAccessToken))
            {
                if (string.IsNullOrWhiteSpace( BotScopes ) || 
                    string.IsNullOrWhiteSpace( RedirectUrl ) ||
                    string.IsNullOrWhiteSpace( ClientId ) || 
                    string.IsNullOrWhiteSpace( ClientSecret ))
                {
                    Logger?.LogDebug( "Bot messages not available" );
                    if (_socketModeAvailable)
                        Logger?.LogInformation("Bot is required for SocketMode");
                    _socketModeAvailable = false;
                }
                else
                    _getBotAccessToken = true;
            }
            if (string.IsNullOrWhiteSpace(UserAccessToken))
            {
                if (string.IsNullOrWhiteSpace( UserScopes )
                 || string.IsNullOrWhiteSpace( RedirectUrl )
                 || string.IsNullOrWhiteSpace( ClientId )
                 || string.IsNullOrWhiteSpace( ClientSecret ))
                {
                    Logger?.LogDebug( "User messages not available" );
                }
                else
                    _getUserAccessToken = true;
            }

            if (_getBotAccessToken || _getUserAccessToken)
            {
                if (string.IsNullOrWhiteSpace(ClientId) || 
                    string.IsNullOrWhiteSpace(ClientSecret))
                {
                    Logger?.LogError("Valid ClientId and ClientSecret must be supplied");

                    return false;
                }
            }

            return true;
        }

        public async Task<bool> Connect()
        {
            if (LogBuilder != null)
                Logger = LoggerFactory.Create( LogBuilder ).CreateLogger<SlackApp>();

            RedirectUrl ??= SlackConstants.DefaultRedirectUrl;

            _slackClient = new SlackClient( Logger );

            if (!IsValid())
                return false;

            if (_getBotAccessToken || _getUserAccessToken)
            {
                var success = await GetSomeAccessTokens();

                if (!success)
                    return false;
            }

            OnMessage< HelloResponse >( OnHello );
            OnMessage< Goodbye >( ( _, msg ) => Reconnect() );

            OnMessage< AppHomeOpened >( OnHomepageOpened );
            OnMessage< GlobalShortcut >( OnGlobalShortcut );
            OnMessage< MessageShortcut >( OnMessageShortcut );
            OnMessage< SlashCommand >( OnSlashCommand );
            OnMessage< BlockActions >( OnBlockActions );
            OnMessage< BlockSuggestion>( OnBlockSuggestions );
            OnMessage< EventCallback >( OnEventCallback );
            OnMessage< Disconnect >( OnDisconnect );
            OnMessage< Message >( OnAnyMessage );

            // Establish websocket connection
            if (_socketModeAvailable && !string.IsNullOrWhiteSpace(AppLevelToken))
            {
                _slackSocket = new SlackSocket( _slackClient, Logger, this )
                               {
                               #if DEBUG_SHORT_CONNECTIONS
                                   ShortConnections = true  // Useful for debugging websocket closures
                               #endif
                               };
                _slackSocket.ApiEventReceived += OnApiEventRecieved;
                await _slackSocket.ConnectWebSocket(AppLevelToken!);
            }

            return true;
        }

        private async void OnDisconnect( ISlackApp app, Disconnect msg )
        {
            _slackSocket?.CloseWebSocket();
            
            if (_slackSocket != null)
                await _slackSocket.ConnectWebSocket(AppLevelToken!);
        }

        private void OnEventCallback( ISlackApp app, EventCallback msg )
        {
            if (msg.payload == null) return;
            
            AppId  = msg.payload.api_app_id;
            TeamId = msg.payload.team_id;
            
            if (msg.payload.@event != null)
                RaiseApiEvent( msg.payload.@event, msg.payload );
        }

        private void OnBlockActions(ISlackApp slackApp, BlockActions msg)
        {
            var surface    = FindSurface( msg.view, msg.message );

            surface?.Process(msg);
        }
        private void OnBlockSuggestions(ISlackApp slackApp, BlockSuggestion msg, IEnvelope<SlackMessage> envelope)
        {
            var surface = FindSurface(msg.view, msg.message); 

            surface?.Process(msg, envelope.envelope_id);
        }

        private async void OnHomepageOpened(ISlackApp slackApp, AppHomeOpened msg)
        {
            if (_hometabType == null) return;
            if (_hometabView != null) return;

            _hometabView = (SlackSurface?)Activator.CreateInstance(_hometabType, this);

            if (_hometabView == null) return;

            _surfaces.Add( _hometabView );
            await PublishHomepage( _hometabView, msg );
        }

        private void OnGlobalShortcut(ISlackApp slackApp, GlobalShortcut shortcut, IEnvelope<SlackMessage> envelope)
        {
            var parser = FindParser(shortcut.callback_id, out List<string>? parameters);
            if (parser == null) return;

            try
            {
                var type = parser.CommandType;
                var obj  = (SlackGlobalShortcutCommand?)Activator.CreateInstance(type);

                if (obj == null)
                    return;

                obj.SlackApp = slackApp;
                obj.Shortcut = shortcut;

                var success = parser.ParseArguments(obj, args: parameters);

                if (success)
                    return;
            }
            catch (ArgumentException aex)
            {
                Logger.LogError(aex, "Processing Message");
                Say(aex.ToString(), channel: shortcut.user.id);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Processing Message");
                Say(e.ToString(), channel: shortcut.user.id);
            }

            var help = parser.Help();
            Say("```\n" + help + "\n```", channel: shortcut.user.id);
        }

        private void OnMessageShortcut(ISlackApp slackApp, MessageShortcut shortcut)
        {
            var parser = FindParser(shortcut.callback_id, out List<string>? parameters);
            if (parser == null) return;

            try
            {
                var type = parser.CommandType;
                var obj  = (SlackMessageShortcutCommand?)Activator.CreateInstance(type);

                if (obj == null)
                    return;

                obj.SlackApp = slackApp;
                obj.Shortcut = shortcut;

                var success = parser.ParseArguments(obj, args: parameters);

                if (success)
                    return;
            }
            catch (ArgumentException aex)
            {
                Logger.LogError(aex, "Processing Message");
                Say(aex.ToString(), channel: shortcut.user.id);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Processing Message");
                Say(e.ToString(), channel: shortcut.user.id);
            }

            var help = parser.Help();
            Say("```\n" + help + "\n```", channel: shortcut.user.id);

        }

        private void OnSlashCommand(ISlackApp slackApp, SlashCommand slashCommand, IEnvelope<SlackMessage> envelope)
        {
            if (string.IsNullOrWhiteSpace(slashCommand.text))
                return;

            if (string.Equals("help", slashCommand.text, StringComparison.OrdinalIgnoreCase) || slashCommand.text == "?")
            {
                Say("Available Commands\n\n```" + CommandHelp() + "```",
                    slashCommand.channel_id ?? "",
                    slashCommand.user_id);
                return;
            }

            var parser = FindParser(slashCommand.command + " " + slashCommand.text, out List<string>? parameters);
            if (parser == null) return;

            try
            {
                var type = parser.CommandType;
                var obj  = (SlackSlashCommand?)Activator.CreateInstance(type);

                if (obj == null)
                    return;

                obj.SlackApp = slackApp;
                obj.Message  = slashCommand;
                obj.Envelope = envelope;

                var success = parser.ParseArguments(obj, args: parameters);

                if (success)
                    return;
            }
            catch (ArgumentException aex)
            {
                Logger.LogError(aex, "Processing Message");
                Say(aex.ToString(), slashCommand.channel_id, user: slashCommand.user_id );
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Processing Message");
                Say(e.ToString(), slashCommand.channel_id, user: slashCommand.user_id);
            }

            var help = parser.Help();
            Say("```\n" + help + "\n```", slashCommand.channel_id, user: slashCommand.user_id);
        }

        async Task<bool> GetSomeAccessTokens()
        {
            if (_slackClient == null || RedirectUrl == null) 
                return false;
            
            Logger?.LogInformation("Opening Browser to retrieve Slack access tokens");

            var service = new OAuthService(Logger);
            await service.RetrieveTokens(ClientId!, RedirectUrl, BotScopes, UserScopes);

            if (string.IsNullOrWhiteSpace(service.Code))
            {
                Logger?.LogError($"Incorrect authorisation");

                return false;
            }
            Logger?.LogInformation("Retrieving Slack access tokens");
            _oauth = await _slackClient.OAuthAccess(ClientId!, ClientSecret!, service.Code, RedirectUrl!);

            if (_oauth == null)
                return false;
            
            MessageTypes.RegisterAll();

            UserAccessToken = _oauth.authed_user?.access_token;
            BotAccessToken  = _oauth.access_token;

            AccessTokensUpdated?.Invoke(this);

            return true;
        }

        public void RaiseApiEvent< TContainer >( SlackMessage msg, TContainer container ) where TContainer : SlackMessage
        {
            if (string.IsNullOrWhiteSpace(msg.type))
                return;

            foreach (var handler in _apiEventHandlers)
            {
                if (msg is MessageBase messageBase && handler.Filter != null)
                {
                    if (messageBase.text != null && handler.Filter.IsMatch(messageBase.text))
                        handler.MessageAction?.Invoke(this, messageBase);
                }

                if (handler.MessageType    == msg.type &&
                    handler.MessageSubType == msg.subtype)
                {
                    if (handler.MessageContainerAction != null)
                        handler.MessageContainerAction?.Invoke(this, msg, container);
                    else
                        handler.MessageAction?.Invoke(this, msg);
                }
            }

        }

        /// <summary>
        /// Call this when a Interactive Message or API event is raised in your SlackBot API
        /// </summary>
        public void OnApiEventRecieved( SlackMessage msg )
        {
            if (string.IsNullOrWhiteSpace(msg.type)) 
                return;

            if (msg is IEnvelope<SlackMessage> envelope && envelope.payload != null)
            {
                var payload = envelope.payload;

                foreach (var handler in _apiEventHandlers)
                {
                    if (payload is MessageBase messageBase && handler.Filter != null)
                    {
                        if (messageBase.text != null && handler.Filter.IsMatch(messageBase.text))
                            handler.MessageAction?.Invoke(this, messageBase);
                    }

                    if (handler.MessageType == payload.type &&
                        handler.MessageSubType == payload.subtype)
                    {
                        if (handler.MessageContainerAction != null)
                            handler.MessageContainerAction?.Invoke(this, payload, msg);
                        else
                            handler.MessageAction?.Invoke(this, payload);
                    }
                }
            }

            foreach (var handler in _apiEventHandlers)
            {
                if (msg is MessageBase messageBase && handler.Filter != null)
                {
                    if (messageBase.text != null && handler.Filter.IsMatch( messageBase.text )) 
                        handler.MessageAction?.Invoke( this, messageBase );
                }

                if (handler.MessageType == msg.type && 
                    handler.MessageSubType == msg.subtype)
                {
                    handler.MessageAction?.Invoke( this, msg );
                }
            }
        }

        /// <summary>
        /// Register callback when a message matches a regex expression
        /// </summary>
        public void OnMessage( [ NotNull ] string regex, [ NotNull ] Action< ISlackApp, MessageBase > action )
        {
            _apiEventHandlers.Add( new ApiEventHandler(
                                             Filter: new Regex( regex ),
                                             MessageAction: (_, msg) => action( this, (MessageBase)msg ) ) );
        }

        /// <summary>
        /// Register callback for a particular type of message
        /// </summary>
        public void OnMessage<T>([NotNull] Action<ISlackApp,T> action) where T : SlackMessage
        {
            var attr = MessageTypes.GetMessageAttributes<T>();
            if (attr == null) return;

            _apiEventHandlers.Add( new ApiEventHandler(
                                         MessageType: attr.Type,
                                         MessageSubType: attr.SubType,
                                         MessageAction: (_, msg) => action( this, (T)msg ) ) );
        }

        /// <summary>
        /// Register callback for a particular type of message
        /// </summary>
        public void OnMessage<T>([NotNull] Action<ISlackApp, T, IEnvelope<SlackMessage> > action) 
            where T : SlackMessage
        {
            var attr = MessageTypes.GetMessageAttributes<T>();
            if (attr == null) return;

            _apiEventHandlers.Add(new ApiEventHandler(
                  MessageType: attr.Type,
                  MessageSubType: attr.SubType,
                  MessageContainerAction:(_, msg, envelope) => action(this, (T)msg, (IEnvelope<SlackMessage>)envelope)));
        }

        /*
        /// <summary>
        ///     Register action to be called when a command is executed
        /// </summary>
        /// <param name="command">The name of the command. e.g. /blah </param>
        /// <param name="action"></param>
        public void OnSlashCommand( [NotNull] string command, Action< ISlackApp, SlashCommand, SlashCommandsEnvelope > action )
        {
            if (string.IsNullOrEmpty( command )) throw new ArgumentException( "Invalid command", nameof(command) );
            if (!command.StartsWith( "/" ))
                command = "/" + command;

            var attr = MessageTypes.GetMessageAttributes<SlashCommand>();
            if (attr == null) return;

            _apiEventHandlers.Add(new ApiEventHandler(
                  MessageType: attr.Type,
                  MessageSubType: attr.SubType,
                  MessageContainerAction: (_, msg, envelope) =>
                                          {
                                              var slashCommand = (SlashCommand)msg;
                                              if (string.Equals( slashCommand.command, command, StringComparison.OrdinalIgnoreCase ))
                                                  action( this, (SlashCommand)msg, (SlashCommandsEnvelope)envelope );
                                          } ));

        }

        public void OnSlashCommand<TCommand>([NotNull] string command, Action<ISlackApp, TCommand, SlashCommandsEnvelope> action)
            where TCommand : new()
        {
            if (string.IsNullOrEmpty(command)) throw new ArgumentException("Invalid command", nameof(command));
            if (!command.StartsWith("/"))
                command = "/" + command;

            var attr = MessageTypes.GetMessageAttributes<SlashCommand>();
            if (attr == null) return;

            _apiEventHandlers.Add(new ApiEventHandler(
                MessageType: attr.Type,
                MessageSubType: attr.SubType,
                MessageContainerAction: (_, msg, envelope) =>
                {
                    var slashCommand = (SlashCommand)msg;
                    if (string.Equals( slashCommand.command, command, StringComparison.OrdinalIgnoreCase ))
                    {
                        var parser  = new ParamParser<TCommand>();
                        var example = parser.ParseArguments(slashCommand.text, includeEnvironmentVariables: false);

                        action( this, example, (SlashCommandsEnvelope)envelope );
                    }
                }));

        }
        */

        /*
        /// <summary>
        ///     Register action to be called when a Global Shortcut is triggered
        /// </summary>
        public void OnGlobalShortcut( string callbackId, Action< ISlackApp, GlobalShortcut> action )
        {
            if (string.IsNullOrEmpty(callbackId)) throw new ArgumentException("Invalid callback", nameof(callbackId));

            var attr = MessageTypes.GetMessageAttributes<GlobalShortcut>();
            if (attr == null) return;

            _apiEventHandlers.Add( new ApiEventHandler(
               MessageType: attr.Type,
               MessageSubType: attr.SubType,
               MessageContainerAction: ( _, msg, envelope ) =>
                   {
                       var shortcut = (GlobalShortcut)msg;
                       if (string.Equals( shortcut.callback_id, callbackId, StringComparison.OrdinalIgnoreCase ))
                           action( this, shortcut );
                   } ) );
        }
        */
        /*
        /// <summary>
        ///     Register action to be called when a Global Shortcut is triggered
        /// </summary>
        public void OnMessageShortcut(string callbackId, Action<ISlackApp, MessageShortcut> action)
        {
            if (string.IsNullOrEmpty(callbackId)) throw new ArgumentException("Invalid callback", nameof(callbackId));

            var attr = MessageTypes.GetMessageAttributes<MessageShortcut>();
            if (attr == null) return;

            _apiEventHandlers.Add( new ApiEventHandler(
               MessageType: attr.Type,
               MessageSubType: attr.SubType,
               MessageContainerAction: ( _, msg, envelope ) =>
                   {
                       var shortcut = (MessageShortcut)msg;
                       if (string.Equals( shortcut.callback_id, callbackId, StringComparison.OrdinalIgnoreCase ))
                           action( this, shortcut );
                   } ) );
        }
        */


        record CommandHandler( 
            string Command, 
            Action< ISlackApp, SlashCommand > CommandAction );

        record ApiEventHandler( string?                                          MessageType            = null,
                                string?                                          MessageSubType         = null,
                                Regex?                                           Filter                 = null,
                                Action< ISlackApp, SlackMessage >?               MessageAction          = null,
                                Action< ISlackApp, SlackMessage, SlackMessage >? MessageContainerAction = null );

        public void RegisterHometabSurface< T >()
            where T : SlackSurface
        {
            if (_hometabView != null)
                _surfaces.Remove( _hometabView );
            _hometabView = null;
            _hometabType = typeof(T);
        }

        public void RegisterMessageCommand< T >()
            where T : SlackMessageCommand, new()
        {
            _paramParsers.Add( new ParamParser< T >() );
        }

        public void RegisterSlashCommand< T >( ) 
            where T : SlackSlashCommand, new()
        {
            _paramParsers.Add( new ParamParser< T >() );
        }

        public void RegisterGlobalShortcutCommand<T>()
            where T : SlackGlobalShortcutCommand, new()
        {
            _paramParsers.Add(new ParamParser<T>());
        }

        public void RegisterMessageShortcutCommand<T>()
            where T : SlackMessageShortcutCommand, new()
        {
            _paramParsers.Add(new ParamParser<T>());
        }

        public void RegisterSurface( SlackSurface surface )
        {
            _surfaces.Add( surface );
        }

        public string CommandHelp()
        {
            var sb  = new StringBuilder();
            var max = 0;

            // Message Commands
            foreach (var paramParser in _paramParsers)
            {
                var names = string.Join('|', paramParser.CommandNames);
                max = Math.Max( max, names.Length );
            }
            
            foreach (var paramParser in _paramParsers)
            {
                var names = string.Join('|', paramParser.CommandNames);
                sb.AppendLine($"{names.PadRight( max )}  {paramParser.CommandDescription}");
            }

            return sb.ToString();
        }

        public async Task<ViewsPublishResponse?> PublishHomepage( SlackSurface hometab, AppHomeOpened msg )
        {
            string key = $"home_{msg.user}";

            if (_surfaces.Exists( s => s.ExternalId == key ))
                return default;
            
            if (hometab.Layouts.Count > 0)
            {
                var pub = new ViewsPublish
                          {
                              user_id = msg.user,
                              view = new HometabView
                                     {
                                         external_id = key,
                                         callback_id = key,
                                         blocks          = hometab.Layouts
                                     }

                          };
                var result = await Send<ViewsPublish, ViewsPublishResponse>(pub);
                if (result?.ok == true)
                {
                    hometab.ViewId     = result.view.id;
                    hometab.AppId      = result.view.app_id;
                    hometab.BotId      = result.view.bot_id;
                    hometab.TeamId     = result.view.team_id;
                    hometab.CallbackId = result.view.callback_id;
                    hometab.ExternalId = result.view.external_id;
                    hometab.Hash       = result.view.hash;
                    hometab.RootViewId = result.view.root_view_id;

                    if (result.view.blocks != null)
                    {
                        // Update block_id's
                        for (int i = 0; i < result.view.blocks.Count; i++)
                        {
                            var slackView = result.view.blocks[ i ];
                            var layout    = hometab.Layouts[ i ];
                            if (string.IsNullOrEmpty( layout.block_id ))
                                layout.block_id = slackView.block_id;
                        }
                    }

                    if (result.view?.state != null)
                        hometab.UpdateState(result.view?.state?.values);

                    RegisterSurface( hometab );
                }
            }
            return default;
        }

        public async Task OpenModal( ModalView view, string triggerId )
        {
            if (string.IsNullOrEmpty( triggerId ))
                throw new ArgumentException( "TriggerId must contain a value" );
            if (view == null)
                throw new ArgumentException("View must contain a value");

            var response = await Send< ViewOpen, ViewResponse >( new ViewOpen
                                                           {
                                                               trigger_id = triggerId, 
                                                               view = view
                                                           } );
            if (response?.ok == true)
            {
                var surface = new SlackSurface( this ) { View = view };
                if (response?.view.state != null)
                    surface.UpdateState( response.view.state );

                RegisterSurface( surface );
            }
        }

        public async Task OpenSurface( SlackSurface surface, string channel, string? user = null, string? ts = null)
        {
            var response = await Say( surface.Layouts, channel: channel, user: user, ts: ts );

            if (response?.ok == true)
            {
                if (response is ChatPostMessageResponse chatResponse)
                {
                    surface.ts      = chatResponse.ts;
                    surface.message = chatResponse.message;

                    RegisterSurface( surface );
                }
            }
        }

        public async void Update( SlackSurface surface )
        {
            var pub = new ViewUpdate
                      {
                          hash        = surface.Hash,
                          view_id     = surface.ViewId,
                          view = new HometabView
                                 {
                                     callback_id  = surface.CallbackId,
                                     external_id  = surface.ExternalId,
                                     blocks       = surface.Layouts
                                 }
                      };
            var result = await Send<ViewUpdate, ViewResponse>(pub);
            if (result?.ok == true)
            {
                surface.Hash   = result.view.hash;
                surface.ViewId = result.view.id;
            }
        }

        private void OnAnyMessage(ISlackApp app, Message msg)
        {
            if (msg.bot_profile != null)
            {
                Logger.LogTrace( $"Received a message from bot {msg.bot_profile.name}" );
                // This came from a bot so do not action it
                return;
            }

            if (string.Equals( "help", msg.text, StringComparison.OrdinalIgnoreCase ) || msg.text == "?")
            {
                Say( "Available Commands\n\n```" + CommandHelp() + "```",
                    msg.channel ?? "",
                    msg.user );
                return;
            }
            
            var parser = FindParser(msg.text, out List<string>? parameters);

            if (parser == null) return;

            try
            {
                var type = parser.CommandType;
                var obj  = (SlackMessageCommand?)Activator.CreateInstance(type);

                if (obj == null)
                    return;

                obj.SlackApp = app;
                obj.Message  = msg;

                var success = parser.ParseArguments( obj, args: parameters ); // msg.text, parameters);

                if (success)
                    return;
            }
            catch (ArgumentException aex)
            {
                Logger.LogError(aex, "Processing Message");
                Say(aex.ToString(), msg.channel, ts: msg.thread_ts ?? msg.event_ts);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Processing Message");
                Say(e.ToString(), msg.channel, ts: msg.thread_ts ?? msg.event_ts);
            }

            var help = parser.Help();
            Say("```\n" + help + "\n```", msg.channel, ts: msg.ts);
        }

        SlackSurface? FindSurface(View? view, MessageBase? message)
        {
            if (view != null)
                return _surfaces.FirstOrDefault(s => s.CallbackId == view.callback_id);
            else if (message != null)
                return _surfaces.FirstOrDefault(s => s.ts == message.ts);
            return default;
        }

        IParamParser? FindParser(string? text, out List<string>? parameters)
        {
            parameters = null;
            if (string.IsNullOrWhiteSpace(text))
                return null;

            var args = text.ParseArguments();

            if (args == null || args.Count == 0) return null;

            var command = args[0];

            foreach (var parser in _paramParsers)
            {
                if (parser.CommandNames.Contains(command))
                {
                    parameters = args.Skip(1)
                                     .ToList();

                    return parser;
                }

                if (parser.CommandPattern?.IsMatch(text) ?? false)
                {
                    return parser;
                }
            }

            return null;
        }

        void OnHello(ISlackApp app, HelloResponse hello)
        {
            if (_slackClient == null)
                return;

            AppId = hello.connection_info?.app_id ?? "";
        }

        /// <summary>
        /// Send text to a channel.
        /// If you specify a user, then only that user will see the message
        /// To reply to a particular message, pass the thread timestamp
        /// </summary>
        public Task<MessageResponse?> Say(string text,
                                             string           channel,
                                             string?          user = null,
                                             string?          ts   = null)
        {
            return string.IsNullOrWhiteSpace(user)
                       ? SayToChannel(text, null, channel, ts)
                       : SayToUser(text, null, channel, user, ts);
        }

        /// <summary>
        /// Send text to a channel.
        /// If you specify a user, then only that user will see the message
        /// To reply to a particular message, pass the thread timestamp
        /// </summary>
        public Task<MessageResponse?> Say( List<Layout> blocks,
                                             string                channel,
                                             string?               user = null,
                                             string?               ts   = null)
        {
            return string.IsNullOrWhiteSpace(user)
                       ? SayToChannel(null, blocks, channel, ts)
                       : SayToUser("", blocks, channel, user, ts);
        }

        public Task<MessageResponse?> Say(Layout block,
                                             string          channel,
                                             string?          user = null,
                                             string?          ts   = null)
        {
            return Say(new List<Layout> { block }, channel, user, ts);
        }

        public async Task<MessageResponse?> SayToChannel(string?      text,
                                                            List<Layout>? blocks,
                                                            string       channel,
                                                            string?      threadTs = null)
        {
            if (_slackClient == null)
                return default;
            
            if (blocks != null) 
                text = BlockHelpers.ExtractText(blocks);
            
            var chat = new ChatPostMessage
                       {
                           channel = channel, 
                           thread_ts = threadTs, 
                           text = text ?? "", 
                           blocks = blocks
                       };

            var token = GetAccessTokenFor< ChatPostMessage >();
            var response = await _slackClient.Post<ChatPostMessage, ChatPostMessageResponse>(token, chat);
            if (response == null || response.ok == false)
            {
                Logger.LogError( JsonConvert.SerializeObject( response, Formatting.Indented ) );
            }

            return response;
        }

        public async Task<MessageResponse?> SayToUser(string?      text,
                                                         List<Layout>? blocks,
                                                         string      channel,
                                                         string       user,
                                                         string?      threadTs = null)
        {
            if (_slackClient == null)
                return default;
            
            if (blocks != null) 
                text = BlockHelpers.ExtractText(blocks);

            var chat = new ChatPostEphemeral
            {
                channel = channel,
                user = user,
                thread_ts = threadTs,
                text = text ?? "",
                blocks = blocks
            };

            var response = await _slackClient.Post<ChatPostEphemeral, ChatPostMessageResponse>(
                    UserAccessToken ?? BotAccessToken ?? "", chat);
            if (response != null && response.ok == false)
            {
                Logger.LogError(JsonConvert.SerializeObject(response));
            }

            return response;
        }

        private string GetAccessTokenFor< T >() where T : SlackMessage
        {
            string? token = null;
            var    msgType = MessageTypes.GetMessageAttributes<T>();

            if (msgType != null)
            {

                if (msgType.ApiType.HasFlag( Msg.AppLevel ) && !string.IsNullOrWhiteSpace( AppLevelToken ))
                    token = AppLevelToken;
                else if (msgType.ApiType.HasFlag( Msg.BotToken ) && !string.IsNullOrWhiteSpace( BotAccessToken ))
                    token = BotAccessToken;
                else if (msgType.ApiType.HasFlag( Msg.UserToken ) && !string.IsNullOrWhiteSpace( UserAccessToken ))
                    token = UserAccessToken;
            }
            return token ?? UserAccessToken ?? BotAccessToken ?? AppLevelToken ?? "";
        }

        public Task<TResponse?> Send<TRequest, TResponse>(TRequest? request = default)
            where TRequest : SlackMessage
            where TResponse : MessageResponse
        {
            if (_slackClient == null) return Task.FromResult( (TResponse?)null );

            var token   = GetAccessTokenFor< TRequest >();
            var msgType = MessageTypes.GetMessageAttributes<TRequest>();

            if (msgType == null) return Task.FromResult((TResponse?)null);

            return msgType.ApiType.HasFlag( Msg.GetMethod )
                       ? Get< TRequest, TResponse >( token, request )
                       : Post<TRequest, TResponse>(token, request);
        }

        public void Push<TRequest>(TRequest? request = default)
        {
            if (_slackClient == null) return;
            if (request      == null) return;

            _slackSocket?.Push( request );
        }

        private async Task<TResponse?> Get<TRequest,TResponse>(string token, TRequest? request = default)
            where TRequest : SlackMessage
            where TResponse : MessageResponse
        {
            if (_slackClient == null) return default;

            var response = await _slackClient.GetRequest<TRequest,TResponse>( token, request);
            if (response != null && response.ok == false)
            {
                Logger.LogError( JsonConvert.SerializeObject( response ) );
            }

            return response;
        }

        private async Task< TResponse? > Post< TRequest, TResponse >( string token, TRequest? request ) 
            where TRequest : SlackMessage
            where TResponse : MessageResponse
        {
            if (_slackClient == null) return default;

            var response = await _slackClient.Post<TRequest, TResponse>(
                    token, request);
            if (response != null && response.ok == false)
            {
                Logger.LogError(JsonConvert.SerializeObject(response));
            }

            return response;
        }

        /// <summary>
        /// Return Cached information about a user
        /// </summary>
        public User? GetUser( string id ) =>
            _users.FirstOrDefault(u => u.id == id);

        /// <summary>
        /// Return cached information about a channel
        /// </summary>
        public Channel? GetChannel( string id ) =>
            _channels.FirstOrDefault(c => c.id == id);

        /// <summary>
        /// Called when socket is quiet for too long.  Need to reconnect
        /// </summary>
        public Task Reconnect()
        {
            if (_slackSocket == null)
                return Task.CompletedTask;
            
            _slackSocket.CloseWebSocket();

            return _slackSocket.ConnectWebSocket(AppLevelToken);

        }
    }
}

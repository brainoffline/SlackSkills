using System;
using System.Collections.Generic;
using System.Globalization;
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

using SlackForDotNet.Context;
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

        private bool                  _socketModeAvailable;
        private bool                  _getBotAccessToken;
        private bool                  _getUserAccessToken;
        private SlackClient?          _slackClient;
        private SlackSocket?          _slackSocket;
        private ContextManager?       _contextManager;
        private OAuthAccessResponse2? _oauth;
        private SlackSurface?         _hometabView = null;
        private Type?                 _hometabType;


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
            OnMessage< ViewSubmission>( OnViewSubmission );
            OnMessage< ViewClosed >( OnViewClosed );
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
            if (msg.payload?.@event != null)
                RaiseApiEvent( msg.payload.@event, msg.payload );
        }

        private void OnBlockActions(ISlackApp slackApp, BlockActions msg)
        {
            var surface    = FindSurface( msg.view, msg.message, msg.container );

            surface?.Process(msg);
        }
        private void OnBlockSuggestions(ISlackApp slackApp, BlockSuggestion msg, IEnvelope<SlackMessage> envelope)
        {
            var surface = FindSurface(msg.view, msg.message); 

            surface?.Process(msg, envelope.envelope_id);
        }

        private void OnViewSubmission(ISlackApp slackApp, ViewSubmission msg, IEnvelope<SlackMessage> envelope)
        {
            var surface = FindSurface(msg.view, null);

            surface?.Process(msg, envelope.envelope_id);
        }

        private void OnViewClosed(ISlackApp slackApp, ViewClosed msg)
        {
            var surface = FindSurface(msg.view, null);

            if (surface != null)
            {
                _surfaces.Remove(surface);
                surface.Process( msg );
            }

        }

        private async void OnHomepageOpened(ISlackApp slackApp, AppHomeOpened msg)
        {
            if (_hometabType == null) return;
            if (_hometabView != null) return;

            _hometabView = (SlackSurface?)Activator.CreateInstance(_hometabType, this);

            if (_hometabView == null) return;

            _surfaces.Add( _hometabView );
            await OpenHomepageSurface( _hometabView, msg );
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
            Say("```\n" + help + "\n```", channel: shortcut.channel.id, shortcut.user.id);

        }

        private void OnSlashCommand(ISlackApp slackApp, SlashCommand slashCommand, IEnvelope<SlackMessage> envelope)
        {
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

        public async Task<ViewsPublishResponse?> OpenHomepageSurface( SlackSurface hometab, AppHomeOpened msg )
        {
            string key = $"home_{msg.user}";

            if (_surfaces.Exists( s => s.View!.external_id == key ))
                return default;

            var layouts = hometab.BuildLayouts();
            
            if (layouts.Count > 0)
            {
                var pub = new ViewsPublish
                          {
                              user_id = msg.user,
                              view = new HometabView
                                     {
                                         external_id = key,
                                         callback_id = key,
                                         blocks      = layouts
                                     }
                          };
                hometab.View ??= pub.view;

                var response = await Send<ViewsPublish, ViewsPublishResponse>(pub);
                if (response?.ok == true)
                {
                    hometab.UpdateStateFrom( response.view );

                    RegisterSurface( hometab );
                }
            }
            return default;
        }

        public async Task OpenModalSurface( SlackSurface surface, string triggerId )
        {
            if (string.IsNullOrEmpty( triggerId ))
                throw new ArgumentException( "TriggerId must contain a value" );

            surface.View ??= new ModalView
                             {
                                 title           = surface.Title,
                                 submit          = "submit",
                                 close           = "close",
                                 notify_on_close = true
                             };
            surface.BuildLayouts();

            var response = await Send< ViewOpen, ViewResponse >( new ViewOpen
                                                                 {
                                                                     trigger_id = triggerId, 
                                                                     view = surface.View.Duplicate()
                                                                 } );
            if (response?.ok == true)
            {
                surface.ResponseMetaData = response.response_metadata;
                surface.UpdateStateFrom( response.view );

                RegisterSurface( surface );
            }
        }

        public async Task OpenMessageSurface( SlackSurface surface, string channelId, string? userId = null, string? ts = null)
        {
            var layouts = surface.BuildLayouts();

            var response = await Say( layouts, channel: channelId, user: userId, ts: ts );

            if (response?.ok == true)
            {
                if (response is ChatPostMessageResponse chatResponse)
                {
                    surface.ts        = chatResponse.message?.ts ?? chatResponse.ts ?? chatResponse.message_ts;
                    surface.channelId = channelId;
                    surface.userId    = userId;
                    surface.message   = chatResponse.message;

                    RegisterSurface( surface );
                }
            }
        }

        public void ModalErrors( string envelopeId, Dictionary< string, string > errors )
        {
            Push(new AcknowledgeResponse<ViewResponseAction>(
                                                             envelopeId,
                                                             new ViewResponseAction
                                                             {
                                                                 response_action = "errors",
                                                                 errors = errors
                                                             }));
        }

        public void UpdateModal( SlackSurface surface, string envelopeId )
        {
            surface.BuildLayouts();

            if (surface.View == null)       // Modals must have views
                return;

            Push( new AcknowledgeResponse< ViewResponseAction >( 
                envelopeId, 
                new ViewResponseAction
                {
                    response_action = "update", 
                    view = surface.View
                } ) );
        }

        public async void Update(SlackSurface surface)
        {
            var layouts = surface.BuildLayouts();

            if (surface.View == null)
            {
                // Message surface
                var update = new ChatUpdate
                             {
                                 channel = surface.channelId,
                                 ts      = surface.ts,
                                 blocks  = layouts
                             };
                var response = await Send<ChatUpdate, ChatResponse>(update);
                if (response?.ok == true)
                {
                }
            }
            else
            {
                var pub = new ViewUpdate
                          {
                              hash    = surface.View.hash,
                              view_id = surface.View.id,
                              view    = surface.View.Duplicate()
                          };

                var response = await Send<ViewUpdate, ViewResponse>(pub);
                if (response?.ok == true)
                {
                    surface.UpdateStateFrom(response.view);
                }
            }
        }


        public async Task RemoveSurface( SlackSurface surface )
        {
            var response = await Send< ChatDelete, MessageResponse >( 
                new ChatDelete { channel = surface.channelId!, ts = surface.ts! } );

            if (_surfaces.Contains( surface ))
                _surfaces.Remove( surface );
        }

        public ContextManager ContextManager =>
            _contextManager ??= new ContextManager( this );

        public Task< UserContext? > GetUserContext( string? userId ) =>
            ContextManager.GetUserContext( userId );

        public Task< ChannelContext? > GetChannelContext( string? channelId ) =>
            ContextManager.GetChannelContext( channelId );

        public Task< BotContext? > GetBotContext( string? botId, string? teamId = null ) =>
            ContextManager.GetBotContext( botId, teamId );
            
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
            Say("```\n" + help + "\n```", msg.channel, msg.user);
        }

        SlackSurface? FindSurface( View? view, MessageBase? message, Container? msgContainer = null )
        {
            if (view != null)
                return _surfaces.FirstOrDefault(s => s.View?.callback_id == view.callback_id);
            if (message != null)
                return _surfaces.FirstOrDefault(s => s.ts == message.ts);
            if (msgContainer != null)
                return _surfaces.FirstOrDefault(s => s.ts == msgContainer.message_ts);
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

        public async Task<MessageResponse?> AddReaction(Message msg, string reaction)
        {
            return await Send<ReactionAdd, MessageResponse>(
                new ReactionAdd { channel = msg.channel, name = reaction, timestamp = msg.ts });
        }
        public async Task<MessageResponse?> RemoveReaction(Message msg, string reaction)
        {
            return await Send<ReactionRemove, MessageResponse>(
                new ReactionRemove { channel = msg.channel, name = reaction, timestamp = msg.ts });
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

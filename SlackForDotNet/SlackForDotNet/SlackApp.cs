using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Asciis;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;

using SlackForDotNet.Surface;
#if SYSTEM_JSON
using JsonSerializer = System.Text.Json.JsonSerializer;
#else
using Newtonsoft.Json;
#endif

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

        private bool                  _socketModeAvailable;
        private bool                  _getBotAccessToken;
        private bool                  _getUserAccessToken;
        private SlackClient?          _slackClient;
        private SlackSocket?          _slackSocket;
        private OAuthAccessResponse2? _oauth;
        private List< User >          _users    = new();
        private List< Channel >       _channels = new ();
        private List< UserGroup >?    _userGroups;


        private readonly List< ApiEventHandler > _apiEventHandlers = new();
        private readonly List< CommandHandler >  _commandHandlers  = new();
        private readonly List< IParamParser >    _paramParsers     = new();
        private readonly List< SlackSurface >    _surfaces         = new();

        static SlackApp()
        {
            MessageTypes.RegisterAll();

#if SYSTEM_JSON
#else
            JsonConvert.DefaultSettings = () =>
                                              new JsonSerializerSettings
                                              {
                                                  Formatting        = Formatting.Indented,
                                                  NullValueHandling = NullValueHandling.Ignore
                                              };
#endif
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

            OnMessage< BlockActions >( OnBlockActions );
            OnMessage< EventCallback >( OnEventsApi );
            OnMessage< Interactive >( OnInteractive );
            OnMessage< Disconnect >( OnDisconnect );
            OnMessage< Message >( OnAnyMessage );

            RegisterCommand< HelpCommand >();

            // Establish websocket connection
            if (_socketModeAvailable && !string.IsNullOrWhiteSpace(AppLevelToken))
            {
                _slackSocket = new SlackSocket( _slackClient, Logger )
                               {
                               #if DEBUG_SHORT_CONNECTIONS
                                   ShortConnections = true
                               #endif
                               };
                _slackSocket.ApiEventReceived += ( sender, message ) => { RaiseApiEvent( message ); };
                await _slackSocket.ConnectWebSocket(AppLevelToken!);

                _slackSocket.SendRequest(new Ping());
            }

            return true;
        }

        private async void OnDisconnect( ISlackApp app, Disconnect msg )
        {
            _slackSocket?.CloseWebSocket();
            
            if (_slackSocket != null)
                await _slackSocket.ConnectWebSocket(AppLevelToken!);
            
            _slackSocket?.SendRequest(new Ping());
        }

        private void OnEventsApi( ISlackApp app, EventCallback msg )
        {
            if (msg.payload == null) return;
            
            AppId  = msg.payload.api_app_id;
            TeamId = msg.payload.team_id;
            
            if (msg.payload.@event != null)
                RaiseApiEvent( msg.payload.@event );
        }

        private void OnInteractive(ISlackApp app, Interactive msg)
        {
            if (msg.payload != null)
                RaiseApiEvent(msg.payload);

        }
        private void OnBlockActions(ISlackApp slackApp, BlockActions msg)
        {
            var callbackId = msg.view?.callback_id;

            var surface = _surfaces.FirstOrDefault( s => s.CallbackId == callbackId );
            surface?.Process( msg );
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
        
        /// <summary>
        /// Call this when a Interactive Message or API event is raised in your SlackBot API
        /// </summary>
        public void RaiseApiEvent( SlackMessage msg )
        {
            if (string.IsNullOrWhiteSpace(msg.type)) 
                return;

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
            _apiEventHandlers.Add( new ApiEventHandler(
                                             MessageType: attr.Type,
                                             MessageSubType: attr.SubType,
                                             MessageAction: (_, msg) => action( this, (T)msg ) ) );
        }

        /// <summary>
        /// Register a Command callback (e.g. /SlackDK with some text)
        /// </summary>
        public void OnCommand( string command, Action< ISlackApp, SlackCommand > action )
        {
            _commandHandlers.Add(new CommandHandler(command, action));
        }
        
        record CommandHandler( 
            string Command, 
            Action< ISlackApp, SlackCommand > CommandAction );

        record ApiEventHandler( string?                     MessageType    = null,
                                string?                     MessageSubType = null,
                                Regex?                      Filter         = null,
                                Action< ISlackApp, SlackMessage >? MessageAction  = null );

        public void RegisterCommand< T >()
            where T : BaseSlackCommand, new()
        {
            _paramParsers.Add( new ParamParser< T >() );
        }

        public void RegisterSurface( SlackSurface surface )
        {
            _surfaces.Add( surface );
        }

        public string CommandHelp()
        {
            var sb  = new StringBuilder();
            var max = 0;
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
                    //hometab.State      = result.view.state;

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

                    RegisterSurface( hometab );
                }
            }
            return default;
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
            
            var parser = FindParser(msg.text, out List<string>? parameters);

            if (parser == null) return;

            try
            {
                var type = parser.CommandType;
                var obj  = (BaseSlackCommand?)Activator.CreateInstance(type);

                if (obj == null)
                    return;

                obj.SlackApp = app;
                obj.Message  = msg;

                var success = parser.ParseArguments(obj, msg.text, parameters);

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
#if SYSTEM_JSON
                Logger.LogError(JsonSerializer.Serialize(response, JsonHelpers.DefaultJsonOptions));
#else
                Logger.LogError( JsonConvert.SerializeObject( response, Formatting.Indented ) );
#endif
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
#if SYSTEM_JSON
                Logger.LogError(JsonSerializer.Serialize(response, JsonHelpers.DefaultJsonOptions));
#else
                Logger.LogError(JsonConvert.SerializeObject(response));
#endif
            }

            return response;
        }

        private string GetAccessTokenFor< T >() where T : SlackMessage
        {
            string token;
            var    msgType = MessageTypes.GetMessageAttributes<T>();
            if (msgType.ApiType.HasFlag(Msg.AppLevel) && !string.IsNullOrWhiteSpace(AppLevelToken))
                token = AppLevelToken;
            else if (msgType.ApiType.HasFlag(Msg.BotToken) && !string.IsNullOrWhiteSpace(BotAccessToken))
                token = BotAccessToken;
            else if (msgType.ApiType.HasFlag(Msg.UserToken) && !string.IsNullOrWhiteSpace(UserAccessToken))
                token = UserAccessToken;
            else
                token = UserAccessToken ?? BotAccessToken ?? AppLevelToken ?? "";

            return token;
        }

        public Task<TResponse?> Send<TRequest, TResponse>(TRequest? request = default)
            where TRequest : SlackMessage
            where TResponse : MessageResponse
        {
            if (_slackClient == null) return Task.FromResult( (TResponse?)null );

            var token   = GetAccessTokenFor< TRequest >();
            var msgType = MessageTypes.GetMessageAttributes<TRequest>();
            return msgType.ApiType.HasFlag( Msg.GetMethod )
                       ? Get< TRequest, TResponse >( token, request )
                       : Post<TRequest, TResponse>(token, request);
        }

        private async Task<TResponse?> Get<TRequest,TResponse>(string token, TRequest? request = default)
            where TRequest : SlackMessage
            where TResponse : MessageResponse
        {
            if (_slackClient == null) return default;

            var response = await _slackClient.GetRequest<TRequest,TResponse>( token, request);
            if (response != null && response.ok == false)
            {
#if SYSTEM_JSON
                Logger.LogError(JsonSerializer.Serialize(response, JsonHelpers.DefaultJsonOptions));
#else
                Logger.LogError( JsonConvert.SerializeObject( response ) );
#endif
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
#if SYSTEM_JSON
                Logger.LogError(JsonSerializer.Serialize(response, JsonHelpers.DefaultJsonOptions));
#else
                Logger.LogError(JsonConvert.SerializeObject(response));
#endif
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

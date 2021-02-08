using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json.Linq;
#if SYSTEM_JSON
using System.Text.Json;
#else
using Newtonsoft.Json;
#endif

using SlackForDotNet.WebApiContracts;

namespace SlackForDotNet
{
    public class SlackSocket
    {
        private readonly ILogger< SlackApp >?         _logger;
        private          SlackClient                  _slackClient;
        private          ClientWebSocket?             _socket;
        private readonly CancellationTokenSource      _socketCancel = new();
        private readonly AutoResetEvent               _resetEvent   = new( false );
        private readonly ConcurrentQueue< string? >   _sendQueue    = new();
        private          AppsConnectionsOpenResponse? _appsConnectionResponse;
        private          int                          _currentId = 1;
        
        public           bool                         ShortConnections { get; set; }

        public event EventHandler<SlackMessage>? ApiEventReceived;

        public SlackSocket( SlackClient slackClient, ILogger< SlackApp >? logger )
        {
            _slackClient = slackClient;
            _logger      = logger;
        }

        public async Task ConnectWebSocket(string? appLevelToken)
        {
            if (appLevelToken == null)
                return;

            _appsConnectionResponse = await _slackClient.Post< AppsConnectionsOpenRequest, AppsConnectionsOpenResponse >(
                                       appLevelToken, new AppsConnectionsOpenRequest() )
                                         .ConfigureAwait(true);

            if (!(_appsConnectionResponse?.ok ?? false))
                throw new Exception("Unable to Connect");

            await StartWebSocket().ConfigureAwait(true);

            StartProcessingSendQueue();
            StartReceivingWebSocketQueue();
        }

        public Task StartWebSocket()
        {
            if (_appsConnectionResponse == null)
                return Task.CompletedTask;
            
            _socket = new ClientWebSocket();

            var uri = ShortConnections
                          ? new Uri( _appsConnectionResponse.url + "&debug_reconnects=true" )
                          : new Uri( _appsConnectionResponse.url );
            return _socket.ConnectAsync(uri, _socketCancel.Token);
        }

        void StartProcessingSendQueue()
        {
            var thread = new Thread(
                () =>
                {
                    try
                    {

                        while (true)
                        {
                            _resetEvent.WaitOne();

                            if (_socket == null)
                                return;
                            
                            while (!_sendQueue.IsEmpty
                                && _socket.State == WebSocketState.Open
                                && !_socketCancel.IsCancellationRequested)
                            {
                                if (_sendQueue.TryDequeue(out string? json))
                                {
                                    if (json == null)
                                        continue;
                                    
                                    _logger?.LogInformation($">>>\n{json}\n");

                                    _socket.SendAsync(
                                        new ArraySegment<byte>(Encoding.UTF8.GetBytes(json)),
                                        WebSocketMessageType.Text,
                                        true,
                                        _socketCancel.Token);
                                }
                                else
                                    break;
                            }

                            if (_socket.State != WebSocketState.Open || _socketCancel.IsCancellationRequested)
                                break;

                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Processing WebSocket send queue");
                    }

                    _logger.LogInformation("Stopped WebSocket send queue");
                });
            thread.Start();
        }

        void StartReceivingWebSocketQueue()
        {
            var thread = new Thread(
                async () =>
                {
                    byte[] bytes = new byte[1024];
                    var buffers = new List<byte[]> { bytes };

                    if (_socket == null)
                        return;

                    while (_socket.State == WebSocketState.Open)
                    {
                        WebSocketReceiveResult wsResult;
                        var buffer = new ArraySegment<byte>(bytes);
                        try
                        {
                            wsResult = await _socket.ReceiveAsync(buffer, _socketCancel.Token).ConfigureAwait(true);
                        }
                        catch (WebSocketException wex)
                        {
                            _logger.LogError(wex, "Processing recieving WebSocket");

                            CloseWebSocket();
                            break;
                        }
                        catch (OperationCanceledException)
                        {
                            _logger.LogInformation("WebSocket Cancelled");
                            continue;
                        }

                        // More of the message to come
                        if (!wsResult.EndOfMessage && buffer.Count == buffer.Array?.Length)
                        {
                            bytes = new byte[1024];
                            buffers.Add(bytes);
                            continue;
                        }

                        var json = string.Join("",
                            buffers.Select(str => Encoding.UTF8.GetString(str).TrimEnd('\0')));
                        bytes = new byte[1024];
                        buffers.Clear();
                        buffers.Add(bytes);

                        HandleMessageJson(json);
                    }

                    _logger.LogInformation("Stopped listening to WebSocket");
                });
            thread.Start();
        }

        /// <summary>
        /// Send event request message
        /// </summary>
        public void SendRequest<TRequest>([NotNull] TRequest message) where TRequest : SlackMessage
        {
            if (message.id == 0)
                message.id = Interlocked.Increment(ref _currentId);

            var messageType                              = MessageTypes.GetMessageAttributes<TRequest>();
            message.type    ??= messageType.Type;
            message.subtype ??= messageType.SubType;

#if SYSTEM_JSON
            var content = JsonSerializer.Serialize(message, JsonHelpers.DefaultJsonOptions);
#else
            var content = JsonConvert.SerializeObject( message );
#endif

            _sendQueue.Enqueue( content );

            KickSendQueue();
        }

        public void Acknowledge( string envelopeId )
        {
            _sendQueue.Enqueue( JsonConvert.SerializeObject(new Acknowledge(envelopeId)) );
            KickSendQueue();
        }

        void HandleMessageJson([NotNull] string json)
        {
            _logger.LogDebug( $"<<<\n{JObject.Parse( json ).ToString( Formatting.Indented )}\n" );

            var msg = MessageTypes.Expand( json );
            if (msg != null)
            {
                if (msg is Envelope envelope)
                    Acknowledge( envelope.envelope_id );
                RaiseEventReceived( msg );
            }
        }

        public void CloseWebSocket()
        {
            try
            {
                if (_socket != null && _socket.State == WebSocketState.Open)
                    _socket.Abort();
                KickSendQueue();
            }
            catch { /* Ignore any exception */ }

        }

        public void KickSendQueue()
        {
            _resetEvent.Set();
        }

        protected virtual void RaiseEventReceived( SlackMessage e )
        {
            ApiEventReceived?.Invoke( this, e );
        }
    }
}

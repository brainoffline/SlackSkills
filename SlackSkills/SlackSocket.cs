using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

using SlackSkills.WebApiContracts;

namespace SlackSkills
{
    public class SlackSocket
    {
        private readonly ILogger< SlackApp >?         _logger;
        private readonly SlackClient                  _slackClient;
        private readonly ISlackApp                    _slackApp;
        private          ClientWebSocket?             _socket;
        private readonly CancellationTokenSource      _socketCancel = new();
        private readonly AutoResetEvent               _resetEvent   = new( false );
        private readonly ConcurrentQueue< string? >   _sendQueue    = new();
        private          AppsConnectionsOpenResponse? _appsConnectionResponse;
        private          int                          _currentId = 1;
        
        public           bool                         ShortConnections { get; set; }

        public event Action< SlackMessage >? ApiEventReceived;

        public SlackSocket( SlackClient slackClient, ILogger< SlackApp >? logger, ISlackApp slackApp )
        {
            _slackClient = slackClient;
            _slackApp    = slackApp;
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
                                    
                                    _logger?.LogInformation($"˄˄˄\n{json}\n");

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
                            _logger.LogError(wex, "Processing receiving WebSocket");

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
                    await _slackApp.Reconnect();
                });
            thread.Start();
        }

        private readonly List< string > _unacknowledgedEnvelopes = new();
        private readonly object         _unacknowledgedLock      = new();

        private void AddEnvelopeId( string envelopeId )
        {
            lock (_unacknowledgedLock)
            {
                if (!_unacknowledgedEnvelopes.Contains( envelopeId ))
                    _unacknowledgedEnvelopes.Add( envelopeId );
            }
        }

        private void RemoveEnvelopeId( string envelopeId )
        {
            lock (_unacknowledgedLock)
            {
                if (_unacknowledgedEnvelopes.Contains(envelopeId))
                    _unacknowledgedEnvelopes.Remove(envelopeId);
            }
        }

        private bool IsUnacknowledgedEnvelopeId( string envelopeId )
        {
            lock (_unacknowledgedLock)
            {
                return _unacknowledgedEnvelopes.Contains( envelopeId );
            }
        }

        /// <summary>
        /// Send event request message
        /// </summary>
        public void PushRequest<TRequest>(TRequest message) where TRequest : SlackMessage
        {
            if (message.id == 0)
                message.id = Interlocked.Increment(ref _currentId);

            var messageType = MessageTypes.GetMessageAttributes< TRequest >();
            if (messageType == null) return;

            message.type    ??= messageType.Type;
            message.subtype ??= messageType.SubType;

            if (message is Envelope envelop)
                RemoveEnvelopeId(envelop.envelope_id);

            var content = JsonConvert.SerializeObject( message );

            _sendQueue.Enqueue( content );

            KickSendQueue();
        }

        public void Push< TRequest >( TRequest message )
        {
            if (message is Envelope envelop)
                RemoveEnvelopeId(envelop.envelope_id);

            var content = JsonConvert.SerializeObject(message);
            _sendQueue.Enqueue(content);
            KickSendQueue();

        }

        public void Acknowledge( string envelopeId, bool force = false )
        {
            if (force || IsUnacknowledgedEnvelopeId( envelopeId ))
            {
                RemoveEnvelopeId( envelopeId );
                _sendQueue.Enqueue( JsonConvert.SerializeObject( new Acknowledge( envelopeId ) ) );
                KickSendQueue();
            }
        }

        void HandleMessageJson(string json)
        {
            _logger.LogDebug( $"www\n{JObject.Parse( json ).ToString( Formatting.Indented )}\n" );

            var msg = MessageTypes.Expand( json );
            if (msg is Envelope envelope)
            {
                if (!envelope.accepts_response_payload)
                    Acknowledge( envelope.envelope_id, force:true );
                else
                    AddEnvelopeId( envelope.envelope_id );
                
                RaiseEventReceived(msg);
                
                if (envelope.accepts_response_payload)
                    Acknowledge( envelope.envelope_id );
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

        protected virtual void RaiseEventReceived( SlackMessage msg )
        {
            ApiEventReceived?.Invoke( msg );
        }
    }
}

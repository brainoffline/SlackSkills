using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;

using SlackForDotNet.Context;
using SlackForDotNet.Surface;
using SlackForDotNet.WebApiContracts;

namespace SlackForDotNet
{
    public interface ISlackApp
    {
        ILogger<SlackApp>? Logger { get; }
        string             AppId  { get; }

        /// <summary>
        /// Send text to a channel.
        /// If you specify a user, then only that user will see the message
        /// To reply to a particular message, pass the thread timestamp
        /// </summary>
        Task<MessageResponse?> Say([NotNull] string text,
                                      string           channel,
                                      string?          user = null,
                                      string?          ts   = null);

        /// <summary>
        /// Send text to a channel.
        /// If you specify a user, then only that user will see the message
        /// To reply to a particular message, pass the thread timestamp
        /// </summary>
        Task<MessageResponse?> Say([NotNull] List<Layout> blocks,
                                      string                channel,
                                      string?               user = null,
                                      string?               ts   = null);

        Task<MessageResponse?> Say([NotNull] Layout block,
                                      string          channel,
                                      string?         user = null,
                                      string?         ts   = null);

        Task<MessageResponse?> SayToChannel(string?      text,
                                               List<Layout>? blocks,
                                               string       channel,
                                               string?      threadTs = null);

        Task<MessageResponse?> SayToUser(string?      text,
                                            List<Layout>? blocks,
                                            string       channel,
                                            string       user,
                                            string?      threadTs = null);

        Task<MessageResponse?> AddReaction(Message msg, string reaction);
        Task<MessageResponse?> RemoveReaction(Message msg, string reaction);

        Task< TResponse? > Send< TRequest, TResponse >( TRequest request ) 
            where TRequest : SlackMessage
            where TResponse : MessageResponse;

        /// <summary>
        ///     Push a response up the WebSocket connection
        /// </summary>
        void Push< TRequest >( TRequest request );

        string CommandHelp();

        Task< ViewsPublishResponse? > OpenHomepageSurface( SlackSurface hometab, AppHomeOpened msg );
        Task                          OpenMessageSurface( SlackSurface     surface, string        channel, string? user = null, string? ts = null );
        Task                          OpenModalSurface( SlackSurface       surface, string        triggerId );
        void                          Update(SlackSurface           surface);
        void                          UpdateModal(SlackSurface      surface,    string envelopeId);
        void                          ModalErrors(string            envelopeId, Dictionary<string,string> errors);
        Task                          RemoveSurface( SlackSurface   surface );


        Task<UserContext?>    GetUserContext( string?   userId );
        Task<ChannelContext?> GetChannelContext(string? channelId);
        Task<BotContext?> GetBotContext(string? botId, string? teamId = null);

        Task Reconnect();
    }
}

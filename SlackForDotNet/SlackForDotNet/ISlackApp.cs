using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;

using SlackForDotNet.Surface;
using SlackForDotNet.WebApiContracts;

namespace SlackForDotNet
{
    public interface ISlackApp
    {
        ILogger<SlackApp>? Logger { get; }
        string             AppId  { get; }
        string             TeamId { get; }

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

        Task< TResponse? > Send< TRequest, TResponse >( TRequest request ) 
            where TRequest : SlackMessage
            where TResponse : MessageResponse;

        /// <summary>
        /// Return Cached information about a user
        /// </summary>
        User? GetUser( string id );

        /// <summary>
        /// Return cached information about a channel
        /// </summary>
        Channel? GetChannel( string id );

        string CommandHelp();

        Task< ViewsPublishResponse? > PublishHomepage( SlackSurface hometab, AppHomeOpened msg );
        void                          Update( SlackSurface          surface );
    }
}

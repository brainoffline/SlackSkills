using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using SlackSkills.Context;
using SlackSkills.Surface;

namespace SlackSkills
{
    public interface ISlackApp
    {
        /// <summary>
        ///     Log messages
        /// </summary>
        ILogger< SlackApp >? Logger { get; }

        /// <summary>
        ///     Slack App Id
        /// </summary>
        string AppId { get; }

        /// <summary>
        ///     Send text to a channel.
        ///     If you specify a user, then only that user will see the message
        ///     To reply to a particular message, pass the thread timestamp
        /// </summary>
        Task< MessageResponse? > Say( string  text,
                                      string  channel,
                                      string? user = null,
                                      string? ts   = null );

        /// <summary>
        ///     Send text to a channel.
        ///     If you specify a user, then only that user will see the message
        ///     To reply to a particular message, pass the thread timestamp
        /// </summary>
        Task< MessageResponse? > Say( List< Layout > blocks,
                                      string         channel,
                                      string?        user = null,
                                      string?        ts   = null );

        /// <summary>
        ///     Send a single layout to be displayed
        /// </summary>
        Task< MessageResponse? > Say( Layout  layout,
                                      string  channel,
                                      string? user = null,
                                      string? ts   = null );

        /// <summary>
        ///     Send text or layouts to a particular channel
        /// </summary>
        Task< MessageResponse? > SayToChannel( string?         text,
                                               List< Layout >? layouts,
                                               string          channel,
                                               string?         threadTs = null );

        /// <summary>
        ///     Send text or a list of blocks as an ephemeral message to a particular user
        /// </summary>
        Task< MessageResponse? > SayToUser( string?         text,
                                            List< Layout >? layouts,
                                            string          channel,
                                            string          user,
                                            string?         threadTs = null );

        /// <summary>
        ///     Add a reaction to a particular message
        /// </summary>
        Task< MessageResponse? > AddReaction( Message msg, string reaction );

        /// <summary>
        ///     Remove a reaction from a message
        /// </summary>
        Task< MessageResponse? > RemoveReaction( Message msg, string reaction );

        /// <summary>
        ///     Send a message to Slack
        /// </summary>
        Task< TResponse? > Send< TRequest, TResponse >( TRequest request )
            where TRequest : SlackMessage
            where TResponse : MessageResponse;

        /// <summary>
        ///     Push a response up the WebSocket connection
        /// </summary>
        void Push< TRequest >( TRequest request );

        /// <summary>
        ///     Respond to input using the response_url variable.
        /// </summary>
        Task Respond< TRequest >( string? url, TRequest request );

        /// <summary>
        ///     Generate help message for all registered message handlers
        /// </summary>
        string CommandHelp();

        /// <summary>
        ///     Open a message surface
        /// </summary>
        Task OpenMessageSurface( SlackSurface surface, string channel, string? user = null, string? ts = null );

        /// <summary>
        ///     Open a modal dialog surface
        /// </summary>
        Task OpenModalSurface( SlackSurface surface, string triggerId );

        /// <summary>
        ///     Update a surface
        /// </summary>
        void Update( SlackSurface surface );

        /// <summary>
        ///     Update modal dialog contents
        /// </summary>
        void UpdateModal( SlackSurface surface, string envelopeId );

        /// <summary>
        ///     Respond to modal dialog input with errors
        /// </summary>
        void ModalErrors( string envelopeId, Dictionary< string, string > errors );

        /// <summary>
        ///     Stops monitoring a surface and attempt to remove the public message surface
        /// </summary>
        Task RemoveSurface( SlackSurface surface );

        /// <summary>
        ///     Get user context for a particular user
        /// </summary>
        Task< UserContext? > GetUserContext( string? userId );

        /// <summary>
        ///     Get channel context for a particular channel
        /// </summary>
        Task< ChannelContext? > GetChannelContext( string? channelId );

        /// <summary>
        ///     Get bot context for a particular bot/team
        /// </summary>
        Task< BotContext? > GetBotContext( string? botId, string? teamId = null );

        /// <summary>
        ///     Called when web socket is quiet for too long.  A connection will be re-established
        /// </summary>
        Task Reconnect();
    }
}

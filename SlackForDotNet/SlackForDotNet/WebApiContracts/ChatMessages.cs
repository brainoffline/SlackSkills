using System;
using System.Collections.Generic;

using SlackForDotNet.Surface;

using static SlackForDotNet.SlackConstants;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

namespace SlackForDotNet.WebApiContracts
{
    [ SlackMessage( "chat.delete",
                      apiType: Msg.BotToken | Msg.UserToken | Msg.Json,
                      scopes: new[] { "chat:write", "chat:write:user", "chat:write:bot" } ) ]
    public class ChatDelete : SlackMessage
    {
        
        public string channel { get; set; }

        
        public string ts { get; set; }

        
        public bool? as_user { get; set; }
    }

    [ SlackMessage( "chat.deleteScheduledMessage",
                      apiType: Msg.BotToken | Msg.UserToken | Msg.Json,
                      scopes: new[] { "chat:write", "chat:write:user", "chat:write:bot" } ) ]
    public class ChatDeleteScheduledMessage : SlackMessage
    {
        
        public string channel { get; set; }

        
        public string scheduled_message_id { get; set; }

        
        public bool? as_user { get; set; }
    }

    [ SlackMessage( "chat.getPermalink",
                      apiType: Msg.FormEncoded | Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "bot" } ) ]
    public class ChatGetPermalink : SlackMessage
    {
        public string channel    { get; set; }
        public string message_ts { get; set; }
    }

    public class ChatGetPermalinkResponse : MessageResponse
    {
        public string channel   { get; set; }
        public string permalink { get; set; }
    }

    [ SlackMessage( "chat.meMessage",
                      apiType: Msg.BotToken | Msg.UserToken | Msg.Json,
                      scopes: new[] { "bot", "chat:write:user" } ) ]
    public class ChatMeMessage : SlackMessage
    {
        
        public string channel { get; set; }

        
        public string text { get; set; }
    }

    [ SlackMessage( "chat.postEphemeral",
                      apiType: Msg.BotToken | Msg.UserToken | Msg.Json,
                      scopes: new[] { BotScopes.Chat_Write, UserScopes.Chat_Write, UserScopes.Chat_Write_User, UserScopes.Chat_Write_Bot } ) ]
    public class ChatPostEphemeral : SlackMessage
    {
        
        public Attachment[] attachments { get; set; }

        
        public string channel { get; set; }

        
        public string text { get; set; }

        
        public string user { get; set; }

        
        public bool? as_user { get; set; }

        
        public List< Layout >? blocks { get; set; }

        
        public string? icon_emoji { get; set; }

        
        public string? icon_url { get; set; }

        
        public bool? link_names { get; set; }

        /// <summary>
        /// Possible Values; none, full
        /// </summary>
        
        public string? parse { get; set; }

        
        public string? thread_ts { get; set; }

        
        public string? username { get; set; }
    }

    [ SlackMessage( "chat.postMessage",
                      apiType: Msg.BotToken | Msg.UserToken | Msg.Json,
                      scopes: new[] { "chat:write", "chat:write:user", "chat:write:bot" } ) ]
    public class ChatPostMessage : SlackMessage
    {
        
        public string channel { get; set; }

        
        public string text { get; set; }

        
        public bool? as_user { get; set; }

        
        public string? user { get; set; }

        
        public Attachment[]? attachments { get; set; }

        
        public List< Layout >? blocks { get; set; }

        
        public string? icon_emoji { get; set; }

        
        public string? icon_url { get; set; }

        
        public bool? link_names { get; set; }

        
        public bool? mrkdwn { get; set; }

        /// <summary>
        /// Possible Values; none, full
        /// </summary>
        
        public string? parse { get; set; }

        public bool? replay_broadcast { get; set; }

        
        public string? thread_ts { get; set; }

        
        public bool? unfurl_links { get; set; }

        
        public bool? unfurl_media { get; set; }

        
        public string? username { get; set; }
    }

    public class ChatPostMessageResponse : MessageResponse
    {
        public string      channel { get; set; }
        public string      ts      { get; set; }
        public MessageBase message { get; set; }
    }

    [ SlackMessage( "chat.scheduleMessage",
                      apiType: Msg.BotToken | Msg.UserToken | Msg.Json,
                      scopes: new[] { "chat:write", "chat:write:user", "chat:write:bot" } ) ]
    public class ChatScheduleMessage : SlackMessage
    {
        
        public string channel { get; set; }

        public int post_at { get; set; }

        
        public string text { get; set; }

        
        public bool? as_user { get; set; }

        
        public string? user { get; set; }

        
        public Attachment[]? attachments { get; set; }

        
        public List<Layout>? blocks { get; set; }

        
        public bool? link_names { get; set; }

        /// <summary>
        /// Possible Values; none, full
        /// </summary>
        
        public string? parse { get; set; }

        public bool? replay_broadcast { get; set; }

        
        public string? thread_ts { get; set; }

        
        public bool? unfurl_links { get; set; }

        
        public bool? unfurl_media { get; set; }
    }

    public class ChatScheduleMessageResponse : MessageResponse
    {
        public string      channel              { get; set; }
        public string      scheduled_message_id { get; set; }
        public string      post_at              { get; set; }
        public MessageBase message              { get; set; }
    }

    [ SlackMessage( "chat.scheduledMessages.list",
                      apiType: Msg.BotToken | Msg.UserToken | Msg.Json,
                      scopes: new[] { "bot" } ) ]
    public class ChatScheduledMessagesList : SlackMessage
    {
        
        public string? channel { get; set; }

        
        public string? cursor { get; set; }

        
        public int? latest { get; set; }

        
        public int? limit { get; set; }

        
        public int? oldest { get; set; }
    }

    public class ChatScheduledMessagesListResponse : MessageResponse
    {
        public MessageBase[] scheduled_messages { get; set; }

        public MetaData response_metadata { get; set; }
    }

    [ SlackMessage( "chat.unfurl",
                      apiType: Msg.UserToken | Msg.Json,
                      scope: "links:write" ) ]
    public class ChatUnfurl : SlackMessage
    {
        
        public string channel { get; set; }

        
        public string ts { get; set; }

        /// <summary>
        /// URL-encoded JSON map with keys set to URLs featured in the the message,
        /// pointing to their unfurl blocks or message attachments
        /// </summary>
        
        public string unfurls { get; set; }

        
        public string? user_auth_message { get; set; }

        
        public bool? user_auth_required { get; set; }

        
        public string? user_auth_url { get; set; }
    }

    [ SlackMessage( "chat.update",
                      apiType: Msg.BotToken | Msg.UserToken | Msg.Json,
                      scopes: new[] { "chat:write", "chat:write:user", "chat:write:bot" } ) ]
    public class ChatUpdate : SlackMessage
    {
        
        public string channel { get; set; }

        
        public string text { get; set; }

        
        public string ts { get; set; }

        
        public bool? as_user { get; set; }

        
        public Attachment[]? attachments { get; set; }

        
        public List<Layout>? blocks { get; set; }

        
        public bool? link_names { get; set; }

        /// <summary>
        /// Possible Values; none, full
        /// </summary>
        
        public string? parse { get; set; }
    }

    public class ChatResponse : MessageResponse
    {
        public string channel { get; set; }
        public string ts      { get; set; }
        public string text    { get; set; }
    }
}

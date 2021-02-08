using System;
using System.Collections.Generic;

using SlackForDotNet.Surface;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

namespace SlackForDotNet
{
    [ SlackMessage( "message", subType: "bot_message", scopes: new[] { "channels:history", "groups:history", "im:history", "mpim:history" } ) ]
    public class MessageBotmessage : MessageBase
    {
        public string       bot_id                { get; set; }
        public bool?        suppress_notification { get; set; }
        public string       username              { get; set; }
        public Bot          bot_profile           { get; set; }
        public List<Layout> blocks                { get; set; }
        public string       user_team             { get; set; }
        public string       source_team           { get; set; }
    }

    /// <summary>
    ///     Undocumented message type
    /// </summary>
    [SlackMessage("message",  "bot_remove")]
    public class MessageBotRemove : MessageBase
    {
        public string bot_id   { get; set; }
        public string bot_link { get; set; }
    }

    /// <summary>
    ///     Help serialize the unknown
    /// </summary>
    [ SlackMessage( "message", subType: "" ) ]
    public class MessageGeneral : MessageBase
    {
        public List<Layout>? blocks            { get; set; }
        public string?       thread_ts         { get; set; }
        public int?          reply_count       { get; set; }
        public int?          reply_users_count { get; set; }
        public string?       latest_reply      { get; set; }
        public string[]?     reply_users       { get; set; }
        public bool?         subscribed        { get; set; }
        public string?       last_read         { get; set; }
    }


    [ SlackMessage( "message", subType: "channel_archive", scopes: new[] { "channels:history", "groups:history", "im:history", "mpim:history" } ) ]
    public class MessageChannelArchive : MessageBase { }

    [ SlackMessage( "message", subType: "channel_join", scopes: new[] { "channels:history", "groups:history", "im:history", "mpim:history" } ) ]
    public class MessageChannelJoin : MessageBase
    {
        public string inviter { get; set; }
    }

    [ SlackMessage( "message", subType: "channel_leave", scopes: new[] { "channels:history", "groups:history", "im:history", "mpim:history" } ) ]
    public class MessageChannelLeave : MessageBase { }

    [ SlackMessage( "message", subType: "channel_name", scopes: new[] { "channels:history", "groups:history", "im:history", "mpim:history" } ) ]
    public class MessageChannelName : MessageBase
    {
        public string name     { get; set; }
        public string old_name { get; set; }
    }

    [ SlackMessage( "message", subType: "channel_purpose", scopes: new[] { "channels:history", "groups:history", "im:history", "mpim:history" } ) ]
    public class MessageChannelPurpose : MessageBase
    {
        public string purpose { get; set; }
    }

    [ SlackMessage( "message", subType: "channel_topic", scopes: new[] { "channels:history", "groups:history", "im:history", "mpim:history" } ) ]
    public class MessageChannelTopic : MessageBase
    {
        public string topic { get; set; }
    }

    [ SlackMessage( "message", subType: "channel_unarchive", scopes: new[] { "channels:history", "groups:history", "im:history", "mpim:history" } ) ]
    public class MessageChannelUnArchive : MessageBase { }

    [ SlackMessage( "message", subType: "ekm_access_denied", scopes: new[] { "channels:history", "groups:history", "im:history", "mpim:history" } ) ]
    public class MessageEkmAccessDenied : MessageBase { }


    [ SlackMessage( "message", subType: "file_comment", scopes: new[] { "channels:history", "groups:history", "im:history", "mpim:history" } ) ]
    public class MessageFileComment : MessageBase
    {
        public SlackFile file    { get; set; }
        public Comment   comment { get; set; }
    }
    [SlackMessage( "message", subType: "file_mention", scopes: new[] { "channels:history", "groups:history", "im:history", "mpim:history" })]
    public class MessageFileMention : MessageBase
    {
        public SlackFile file    { get; set; }
    }
    [SlackMessage( "message", subType: "file_share", scopes: new[] { "channels:history", "groups:history", "im:history", "mpim:history" })]
    public class MessageFileShare : MessageBase
    {
        public List<SlackFile>? files          { get; set; }
        public bool             update         { get; set; }
        public bool             display_as_bot { get; set; }
        public string           bot_id         { get; set; }
    }


    [SlackMessage( "message", subType: "group_archive", scopes: new[] { "channels:history", "groups:history", "im:history", "mpim:history" } ) ]
    public class MessageGroupArchive : MessageBase
    {
        public string[]? members { get; set; }
    }

    [ SlackMessage( "message", subType: "group_join", scopes: new[] { "channels:history", "groups:history", "im:history", "mpim:history" } ) ]
    public class MessageGroupJoin : MessageBase { }

    [ SlackMessage( "message", subType: "group_leave", scopes: new[] { "channels:history", "groups:history", "im:history", "mpim:history" } ) ]
    public class MessageGroupLeave : MessageBase { }

    [ SlackMessage( "message", subType: "group_name", scopes: new[] { "channels:history", "groups:history", "im:history", "mpim:history" } ) ]
    public class MessageGroupName : MessageBase
    {
        public string name     { get; set; }
        public string old_name { get; set; }
    }

    [ SlackMessage( "message", subType: "group_purpose", scopes: new[] { "channels:history", "groups:history", "im:history", "mpim:history" } ) ]
    public class MessageGroupPurpose : MessageBase
    {
        public string purpose { get; set; }
    }

    [ SlackMessage( "message", subType: "group_topic", scopes: new[] { "channels:history", "groups:history", "im:history", "mpim:history" } ) ]
    public class MessageGroupTopic : MessageBase
    {
        public string topic { get; set; }
    }

    [ SlackMessage( "message", subType: "group_unarchive", scopes: new[] { "channels:history", "groups:history", "im:history", "mpim:history" } ) ]
    public class MessageGroupUnArchive : MessageBase
    {
        public string purpose { get; set; }
    }

    [ SlackMessage( "message", subType: "me_message", scopes: new[] { "channels:history", "groups:history", "im:history", "mpim:history" } ) ]
    public class MessageMeMessage : MessageBase { }

    [ SlackMessage( "message", subType: "message_changed", scopes: new[] { "channels:history", "groups:history", "im:history", "mpim:history" } ) ]
    public class MessageChanged : MessageBase
    {
        public bool?          hidden           { get; set; }
        public MessageGeneral message          { get; set; }
        public MessageGeneral previous_message { get; set; }
    }
    [ SlackMessage( "message", subType: "message_deleted", scopes: new[] { "channels:history", "groups:history", "im:history", "mpim:history" } ) ]
    public class MessageDeleted : MessageBase
    {
        public bool?  hidden     { get; set; }
        public string deleted_ts { get; set; }
    }
    [ SlackMessage( "message", subType: "message_replied", scopes: new[] { "channels:history", "groups:history", "im:history", "mpim:history" } ) ]
    public class MessageReplied : MessageBase
    {
        public bool?          hidden  { get; set; }
        public MessageGeneral message { get; set; }
    }

    [SlackMessage( "message", subType: "message_pinned", scopes: new[] { "channels:history", "groups:history", "im:history", "mpim:history" })]
    public class MessagePinned : MessageBase
    {
        public string         item_type { get; set; }
        public MessageGeneral item      { get; set; }
    }
    [SlackMessage( "message", subType: "pinned_item", scopes: new[] { "channels:history", "groups:history", "im:history", "mpim:history" })]
    public class MessagePinnedItem : MessageBase
    {
        public string         item_type { get; set; }
        public MessageGeneral item      { get; set; }
    }

    [SlackMessage( "message", subType: "thread_broadcast", scopes: new[] { "channels:history", "groups:history", "im:history", "mpim:history" } ) ]
    public class MessageThreadBroadcast : MessageBase
    {
        public MessageGeneral message { get; set; }
    }
    
    /// <summary>
    /// Undocumented
    /// </summary>
    [SlackMessage( "message", subType: "tombstone", scopes: new[] { "channels:history", "groups:history", "im:history", "mpim:history" })]
    public class MessageTombstone : MessageBase
    {
    }
    
    [SlackMessage( "message", subType: "unpinned_item", scopes: new[] { "channels:history", "groups:history", "im:history", "mpim:history" } ) ]
    public class MessageUnPinned : MessageBase
    {
        public string         item_type { get; set; }
        public MessageGeneral item      { get; set; }
    }
}

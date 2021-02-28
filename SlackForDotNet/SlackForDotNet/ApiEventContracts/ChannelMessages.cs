using System;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

namespace SlackForDotNet
{
    [ SlackMessage( "channel_archive", apiType: Msg.RTM | Msg.Event, scope: "channels:read" ) ]
    public class ChannelArchive : SlackMessage
    {
        public string channel { get; set; }
        public string user    { get; set; }
    }

    [ SlackMessage( "channel_created", apiType: Msg.RTM | Msg.Event, scope: "channels:read" ) ]
    public class ChannelCreated : SlackMessage
    {
        public Channel channel { get; set; }
    }

    [ SlackMessage( "channel_deleted", apiType: Msg.RTM | Msg.Event, scope: "channels:read" ) ]
    public class ChannelDeleted : SlackMessage
    {
        public string channel { get; set; }
    }

    [ SlackMessage( "channel_history_changed", apiType: Msg.RTM | Msg.Event, scopes: new[] { "channels:history", "groups:history", "mpim:history" } ) ]
    public class ChannelHistoryChanged : SlackMessage
    {
        public string latest   { get; set; }
        public string ts       { get; set; }
    }

    [SlackMessage( "channel_id_changed", apiType: Msg.RTM | Msg.Event, scopes: new[] { "channels:history", "groups:history", "mpim:history" })]
    public class ChannelIdChanged : SlackMessage
    {
        public string old_channel_id { get; set; }
        public string new_channel_id { get; set; }
        public string event_ts       { get; set; }
    }

    [SlackMessage( "channel_joined" ) ]
    public class ChannelJoined : SlackMessage
    {
        public Channel channel { get; set; }
    }

    [ SlackMessage( "channel_left", apiType: Msg.RTM | Msg.Event, scope: "channels:read" ) ]
    public class ChannelLeft : SlackMessage
    {
        public string channel { get; set; }
    }

    [ SlackMessage( "channel_marked" ) ]
    public class ChannelMarked : SlackMessage
    {
        public string channel { get; set; }
        public string ts      { get; set; }
    }

    [ SlackMessage( "channel_rename", apiType: Msg.RTM | Msg.Event, scope: "channels:read" ) ]
    public class ChannelRename : SlackMessage
    {
        public Channel channel { get; set; }
    }

    [ SlackMessage( "channel_shared", apiType: Msg.Event ) ]
    public class ChannelShared : SlackMessage
    {
        public string connected_team_id { get; set; }
        public string channel           { get; set; }
    }

    [ SlackMessage( "channel_unarchive", apiType: Msg.RTM | Msg.Event, scope: "channels:read" ) ]
    public class ChannelUnarchive : SlackMessage
    {
        public string channel { get; set; }
        public string user    { get; set; }
    }

    [ SlackMessage( "channel_unshared", apiType: Msg.Event ) ]
    public class ChannelUnShared : SlackMessage
    {
        public string previously_connected_team_id { get; set; }
        public string channel                      { get; set; }
        public bool?  is_ext_shared                { get; set; }
    }

    [ SlackMessage( "member_joined_channel", apiType: Msg.RTM | Msg.Event, scopes: new[] { "channels:read", "groups:read" } ) ]
    public class MemberJoinedChannel : SlackMessage
    {
        public string user    { get; set; }
        public string channel { get; set; }

        /// <summary>C = typically a public channel, G = private (group) </summary>
        public string channel_type { get; set; }

        public string team    { get; set; }
        public string inviter { get; set; }
    }

    [ SlackMessage( "member_left_channel", apiType: Msg.RTM | Msg.Event, scopes: new[] { "channels:read", "groups:read" } ) ]
    public class MemberLeftChannel : SlackMessage
    {
        public string user    { get; set; }
        public string channel { get; set; }

        /// <summary>C = typically a public channel, G = private (group) </summary>
        public string channel_type { get; set; }

        public string team { get; set; }
    }
}

using System;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

namespace SlackSkills
{
    [ SlackMessage( "group_archive", apiType: Msg.RTM | Msg.Event, scope: "groups:read" ) ]
    public class GroupArchive : SlackMessage
    {
        public string channel { get; set; }
    }

    [ SlackMessage( "group_close", apiType: Msg.RTM | Msg.Event, scope: "groups:read" ) ]
    public class GroupClose : SlackMessage
    {
        public string user    { get; set; }
        public string channel { get; set; }
    }

    [ SlackMessage( "group_deleted", apiType: Msg.RTM | Msg.Event, scope: "groups:read" ) ]
    public class GroupDeleted : SlackMessage
    {
        public string channel { get; set; }
    }

    [ SlackMessage( "group_history_changed", apiType: Msg.RTM | Msg.Event, scope: "groups:history" ) ]
    public class GroupHistoryEventChanged : SlackMessage
    {
        public string latest   { get; set; }
        public string ts       { get; set; }
    }

    [ SlackMessage( "group_joined" ) ]
    public class GroupJoined : SlackMessage
    {
        public Channel channel { get; set; }
    }

    [ SlackMessage( "group_left", apiType: Msg.RTM | Msg.Event, scope: "groups:read" ) ]
    public class GroupLeft : SlackMessage
    {
        public string channel { get; set; }
    }

    [ SlackMessage( "group_marked" ) ]
    public class GroupMarked : SlackMessage
    {
        public string channel { get; set; }
        public string ts      { get; set; }
    }

    [ SlackMessage( "group_open", apiType: Msg.RTM | Msg.Event, scope: "groups:read" ) ]
    public class GroupOpen : SlackMessage
    {
        public string user    { get; set; }
        public string channel { get; set; }
    }

    [ SlackMessage( "group_rename", apiType: Msg.RTM | Msg.Event, scope: "groups:read" ) ]
    public class GroupRename : SlackMessage
    {
        public Channel channel { get; set; }
    }

    [ SlackMessage( "group_unarchive", apiType: Msg.RTM | Msg.Event, scope: "groups:read" ) ]
    public class GroupUnarchive : SlackMessage
    {
        public string channel { get; set; }
    }
}

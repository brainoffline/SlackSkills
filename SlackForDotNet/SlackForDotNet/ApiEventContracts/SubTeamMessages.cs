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
    [ SlackMessage( "subteam_created", apiType: Msg.RTM | Msg.Event, scope: "usergroups:read" ) ]
    public class SubTeamCreated : SlackMessage
    {
        public SubTeam subteam { get; set; }
    }

    [ SlackMessage( "subteam_members_changed", apiType: Msg.RTM | Msg.Event, scope: "usergroups:read" ) ]
    public class SubTeamMembersChanged : SlackMessage
    {
        public string team_id              { get; set; }
        public string subteam_id           { get; set; }
        public int?   date_previous_update { get; set; }
        public int?   date_update          { get; set; }

        public string[]? added_users { get; set; }

        public int?     added_users_count   { get; set; }
        public string[] removed_users       { get; set; }
        public int?     removed_users_count { get; set; }
    }

    [ SlackMessage( "subteam_self_added", apiType: Msg.RTM | Msg.Event, scope: "usergroups:read" ) ]
    public class SubTeamSelfAdded : SlackMessage
    {
        public string subteam_id { get; set; }
    }

    [ SlackMessage( "subteam_self_removed", apiType: Msg.RTM | Msg.Event, scope: "usergroups:read" ) ]
    public class SubTeamSelfRemoved : SlackMessage
    {
        public string subteam_id { get; set; }
    }

    [ SlackMessage( "subteam_updated", apiType: Msg.RTM | Msg.Event, scope: "usergroups:read" ) ]
    public class SubTeamUpdated : SlackMessage
    {
        public SubTeam subteam { get; set; }
    }
}

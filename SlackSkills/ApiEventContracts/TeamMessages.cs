using System;
using System.Collections.Generic;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

namespace SlackSkills
{
    [SlackMessage( "team_access_granted", apiType: Msg.RTM | Msg.Event, scope: "team:read")]
    public class TeamAccessGranted : SlackMessage
    {
        public List<string> team_ids { get; set; }
    }

    [SlackMessage( "team_access_revoked", apiType: Msg.RTM | Msg.Event, scope: "team:read")]
    public class TeamAccessRevoked : SlackMessage
    {
        public List<string> team_ids { get; set; }
    }

    [SlackMessage( "team_domain_change", apiType: Msg.RTM | Msg.Event, scope: "team:read" ) ]
    public class TeamDomainChange : SlackMessage
    {
        public string url    { get; set; }
        public string domain { get; set; }
    }

    [ SlackMessage( "team_join", apiType: Msg.RTM | Msg.Event, scope: "users:read" ) ]
    public class TeamJoin : SlackMessage
    {
        public User user { get; set; }
    }

    [ SlackMessage( "team_migration_started" ) ]
    public class TeamMigrationStarted : SlackMessage { }

    [ SlackMessage( "team_plan_change" ) ]
    public class TeamPlanChange : SlackMessage
    {
        public string plan        { get; set; }
        public bool?  can_add_ura { get; set; }

        
        public string[]? paid_features { get; set; }
    }

    [ SlackMessage( "team_pref_change" ) ]
    public class TeamPrefChange : SlackMessage
    {
        public string name { get; set; }
        public object? value { get; set; }
    }

    [ SlackMessage( "team_profile_change" ) ]
    public class TeamProfileChange : SlackMessage
    {
        public object profile { get; set; }
    }

    [ SlackMessage( "team_profile_delete" ) ]
    public class TeamProfileDelete : SlackMessage
    {
        public object profile { get; set; }
    }

    [ SlackMessage( "team_profile_reorder" ) ]
    public class TeamProfileReorder : SlackMessage
    {
        public object profile { get; set; }
    }

    [ SlackMessage( "team_rename", apiType: Msg.RTM | Msg.Event, scope: "team:read" ) ]
    public class TeamRename : SlackMessage
    {
        public string name { get; set; }
    }
}

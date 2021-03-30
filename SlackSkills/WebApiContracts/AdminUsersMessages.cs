using System;
using System.Collections.Generic;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

namespace SlackSkills.WebApiContracts
{
    [SlackMessage( "admin.usergroups.addChannel",
                     apiType: Msg.UserToken,
                     scope: "admin.usergroups:write")]
    public class AdminUserGroupsAddChannel : SlackMessage
    {
        public string  channel_ids  { get; set; }
        public string  usergroup_id { get; set; }
        public string? team_id      { get; set; }
    }

    [SlackMessage( "admin.usergroups.addTeams",
                     apiType: Msg.UserToken,
                     scope: "admin.teams:write")]
    public class AdminUserGroupsAddTeams : SlackMessage
    {
        public string team_ids       { get; set; }
        public string usergroup_id   { get; set; }
        public bool?  auto_provision { get; set; }
    }

    [SlackMessage( "admin.usergroups.listChannels",
                     apiType: Msg.UserToken,
                     scope: "admin.usergroups:read")]
    public class AdminUserGroupsListChannels : SlackMessage
    {
        public string  usergroup_id        { get; set; }
        public bool?   include_num_members { get; set; }
        public string? team_id             { get; set; }
    }

    public class AdminUserGroupsListChannelsResponse : MessageResponse
    {
        public List<Channel> channels { get; set; }
    }

    [SlackMessage( "admin.usergroups.removeChannels",
                     apiType: Msg.UserToken,
                     scope: "admin.usergroups:write")]
    public class AdminUserGroupsRemoveChannels : SlackMessage
    {
        public string  channel_ids  { get; set; }
        public string? usergroup_id { get; set; }
    }

    /*******************************************************************************/

    [SlackMessage( "admin.users.assign",
                     apiType: Msg.UserToken,
                     scope: "admin.users:write" ) ]
    public class AdminUsersAssign : SlackMessage
    {
        
        public string team_id { get; set; }

        
        public string user_id { get; set; }

        public bool? is_restricted       { get; set; }
        public bool? is_ultra_restricted { get; set; }
    }

    [ SlackMessage( "admin.users.invite",
                      apiType: Msg.UserToken,
                      scope: "admin.users:write" ) ]
    public class AdminUsersInvite : SlackMessage
    {
        
        public string channel_ids { get; set; }

        
        public string email { get; set; }

        
        public string team_id { get; set; }

        public string    custom_message      { get; set; }
        public string    guest_expiration_ts { get; set; }
        public bool?     is_restricted       { get; set; }
        public bool?     is_ultra_restricted { get; set; }
        public SlackName real_name           { get; set; }
        public bool?     resend              { get; set; }
    }

    [SlackMessage( "admin.users.list",
                     apiType: Msg.UserToken,
                     scope: "admin.users:read")]
    public class AdminUsersList : SlackMessage
    {
        public string? cursor  { get; set; }
        public int?    limit   { get; set; }
        public string  team_id { get; set; }
    }

    public class AdminUsersListResponse : MessageResponse
    {
        public List<User> users { get; set; }
    }

    [SlackMessage( "admin.users.remove",
                      apiType: Msg.UserToken,
                      scope: "admin.users:write" ) ]
    public class AdminUsersRemove : SlackMessage
    {
        
        public string team_id { get; set; }

        
        public string user_id { get; set; }
    }

    [SlackMessage( "admin.users.session.invalidate",
                     apiType: Msg.UserToken,
                     scope: "admin.users:write")]
    public class AdminUsersSessionInvalidate : SlackMessage
    {
        public string session_id { get; set; }
        public string team_id { get; set; }
    }

    [SlackMessage( "admin.users.session.list",
                     apiType: Msg.UserToken,
                     scope: "admin.users:read")]
    public class AdminUsersSessionList : SlackMessage
    {
        public string? cursor  { get; set; }
        public int?    limit   { get; set; }
        public string? team_id { get; set; }
        public string? user_id { get; set; }
    }

    public class AdminUsersSessionListResponse : MessageResponse
    {
        public List<Session> active_sessions { get; set; }
    }

    [SlackMessage( "admin.users.session.reset",
                     apiType: Msg.UserToken,
                     scope: "admin.users:write")]
    public class AdminUsersSessionReset : SlackMessage
    {

        public string user_id { get; set; }

        public bool? mobile_only { get; set; }
        public bool? web_only    { get; set; }
    }

    [SlackMessage( "admin.users.setAdmin",
                      apiType: Msg.UserToken,
                      scope: "admin.users:write" ) ]
    public class AdminUsersSetAdmin : SlackMessage
    {
        
        public string team_id { get; set; }

        
        public string user_id { get; set; }
    }

    [SlackMessage( "admin.users.setExpiration",
                     apiType: Msg.UserToken,
                     scope: "admin.users:write")]
    public class AdminUsersSetExpiration : SlackMessage
    {
        public string expiration_ts { get; set; }
        public string user_id       { get; set; }
        public string team_id       { get; set; }
    }

    [SlackMessage( "admin.users.setOwner",
                      apiType: Msg.UserToken,
                      scope: "admin.users:write" ) ]
    public class AdminUsersSetOwner : SlackMessage
    {
        
        public string team_id { get; set; }

        
        public string user_id { get; set; }
    }

    [ SlackMessage( "admin.users.setRegular",
                      apiType: Msg.UserToken,
                      scope: "admin.users:write" ) ]
    public class AdminUsersSetRegular : SlackMessage
    {
        public string team_id { get; set; }
        public string user_id { get; set; }
    }
}

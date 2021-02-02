using System;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

namespace SlackForDotNet.WebApiContracts
{
    [ SlackMessage( "usergroups.create",
                      apiType: Msg.UserToken,
                      scopes: new[] { "usergroups:write" } ) ]
    public class UserGroupCreate : SlackMessage
    {
        
        public string name { get; set; }

        /// <summary>
        /// comma separated list of channel ids
        /// </summary>
        
        public string? channels { get; set; }

        
        public string? description { get; set; }

        
        public string? handle { get; set; }

        public int? include_count { get; set; }
    }

    public class UserGroupResponse : MessageResponse
    {
        public UserGroup usergroup { get; set; }
    }

    [ SlackMessage( "usergroups.disable",
                      apiType: Msg.UserToken,
                      scopes: new[] { "usergroups:write" } ) ]
    public class UserGroupDisable : SlackMessage
    {
        
        public string usergroup { get; set; }

        
        public bool? include_count { get; set; }
    }

    [ SlackMessage( "usergroups.enable",
                      apiType: Msg.UserToken,
                      scopes: new[] { "usergroups:write" } ) ]
    public class UserGroupEnable : SlackMessage
    {
        
        public string usergroup { get; set; }

        
        public bool? include_count { get; set; }
    }

    [ SlackMessage( "usergroups.list",
                      apiType: Msg.FormEncoded | Msg.UserToken,
                      scopes: new[] { "usergroups:read" } ) ]
    public class UserGroupsList : SlackMessage
    {
        public bool? include_count { get; set; }

        public bool? include_disabled { get; set; }

        public bool? include_users { get; set; }
    }

    [ SlackMessage( "usergroups.update",
                      apiType: Msg.UserToken,
                      scopes: new[] { "usergroups:write" } ) ]
    public class UserGroupUpdate : SlackMessage
    {
        
        public string usergroup { get; set; }

        
        public string? name { get; set; }

        /// <summary>
        /// comma separated list of channel ids
        /// </summary>
        
        public string? channels { get; set; }

        
        public string? description { get; set; }

        
        public string? handle { get; set; }

        
        public int? include_count { get; set; }
    }

    [ SlackMessage( "usergroups.users.list",
                      apiType: Msg.FormEncoded | Msg.UserToken,
                      scopes: new[] { "usergroups:read" } ) ]
    public class UserGroupUsersList : SlackMessage
    {
        
        public string usergroup { get; set; }

        public bool? include_disabled { get; set; }
    }

    public class UserGroupUsersListResponse : MessageResponse
    {
        public string[] users { get; set; }
    }

    [ SlackMessage( "usergroups.users.update",
                      apiType: Msg.UserToken,
                      scopes: new[] { "usergroups:write" } ) ]
    public class UserGroupUsersUpdate : SlackMessage
    {
        
        public string usergroup { get; set; }

        /// <summary>
        /// comma separated list of userIds
        /// </summary>
        public string? users { get; set; }

        
        public int? include_count { get; set; }
    }

    public class UsergroupsResponse : MessageResponse
    {
        public UserGroup[] usergroups { get; set; }
    }
}

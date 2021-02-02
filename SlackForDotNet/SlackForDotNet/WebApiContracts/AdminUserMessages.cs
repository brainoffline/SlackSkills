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
    [ SlackMessage( "admin.users.assign",
                      apiType: Msg.UserToken,
                      scope: "admin.users:write" ) ]
    public class AdminUserAssign : SlackMessage
    {
        
        public string team_id { get; set; }

        
        public string user_id { get; set; }

        public bool? is_restricted       { get; set; }
        public bool? is_ultra_restricted { get; set; }
    }

    [ SlackMessage( "admin.users.invite",
                      apiType: Msg.UserToken,
                      scope: "admin.users:write" ) ]
    public class AdminUserInvite : SlackMessage
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

    [ SlackMessage( "admin.users.remove",
                      apiType: Msg.UserToken,
                      scope: "admin.users:write" ) ]
    public class AdminUserRemove : SlackMessage
    {
        
        public string team_id { get; set; }

        
        public string user_id { get; set; }
    }

    [ SlackMessage( "admin.users.session.reset",
                      apiType: Msg.UserToken,
                      scope: "admin.users:write" ) ]
    public class AdminUserSessionReset : SlackMessage
    {
        
        public string user_id { get; set; }

        public bool? mobile_only { get; set; }
        public bool? web_only    { get; set; }
    }

    [ SlackMessage( "admin.users.setAdmin",
                      apiType: Msg.UserToken,
                      scope: "admin.users:write" ) ]
    public class AdminUserSetAdmin : SlackMessage
    {
        
        public string team_id { get; set; }

        
        public string user_id { get; set; }
    }

    [ SlackMessage( "admin.users.setOwner",
                      apiType: Msg.UserToken,
                      scope: "admin.users:write" ) ]
    public class AdminUserSetOwner : SlackMessage
    {
        
        public string team_id { get; set; }

        
        public string user_id { get; set; }
    }

    [ SlackMessage( "admin.users.setRegular",
                      apiType: Msg.UserToken,
                      scope: "admin.users:write" ) ]
    public class AdminUser : SlackMessage
    {
        
        public string team_id { get; set; }

        
        public string user_id { get; set; }
    }
}

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
    [ SlackMessage(
                      "admin.inviteRequests.approve",
                      apiType: Msg.UserToken | Msg.EnterpriseGrid,
                      scope: "admin.invites:write" ) ]
    public class AdminInviteRequestApprove : SlackMessage
    {
        
        public string invite_request_id { get; set; }

        
        public string? team_id { get; set; }
    }

    [ SlackMessage( "admin.inviteRequests.deny",
                      apiType: Msg.UserToken | Msg.EnterpriseGrid,
                      scope: "admin.invites:write" ) ]
    public class AdminInviteRequestDeny : SlackMessage
    {
        
        public string invite_request_id { get; set; }

        
        public string? team_id { get; set; }
    }

    [ SlackMessage( "admin.inviteRequests.list",
                      apiType: Msg.UserToken | Msg.EnterpriseGrid,
                      scope: "admin.invites:read" ) ]
    public class AdminInviteRequestList : SlackMessage
    {
        
        public string? cursor { get; set; }

        
        public int? limit { get; set; }

        
        public string? team_id { get; set; }
    }

    [ SlackMessage( "admin.inviteRequests.approved.list",
                      apiType: Msg.UserToken | Msg.EnterpriseGrid,
                      scope: "admin.invites:read" ) ]
    public class AdminInviteRequestApprovedList : SlackMessage
    {
        
        public string? cursor { get; set; }

        
        public int? limit { get; set; }

        
        public string? team_id { get; set; }
    }

    [ SlackMessage( "admin.inviteRequests.denied.list",
                      apiType: Msg.UserToken | Msg.EnterpriseGrid,
                      scope: "admin.invites:read" ) ]
    public class AdminInviteRequestDeniedList : SlackMessage
    {
        
        public string? cursor { get; set; }

        
        public int? limit { get; set; }

        
        public string? team_id { get; set; }
    }
}

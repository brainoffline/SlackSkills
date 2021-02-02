// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

using System;
using System.Diagnostics.CodeAnalysis;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

namespace SlackForDotNet.WebApiContracts
{
    [ SlackMessage( "admin.apps.approve",
                      apiType: Msg.UserToken | Msg.EnterpriseGrid,
                      scopes: new[] { "admin.apps:write" } ) ]
    public class AdminAppApprove : SlackMessage
    {
        public string? app_id { get; set; }
        public string? request_id { get; set; }
        public string? team_id { get; set; }
    }

    [ SlackMessage( "admin.apps.approved.list",
                      apiType: Msg.FormEncoded | Msg.UserToken | Msg.EnterpriseGrid,
                      scopes: new[] { "admin.apps:read" } ) ]
    public class AdminAppApprovedList : SlackMessage
    {
        public string? cursor { get; set; }
        public string? enterprise_id { get; set; }
        public int? limit { get; set; }
        public string? team_id { get; set; }
    }

    public class ApprovedAppsResponse : MessageResponse
    {
        public AppDetailedInfo[] approved_apps     { get; set; }
        public MetaData          response_metadata { get; set; }
    }

    [ SlackMessage( "admin.apps.request.list",
                      apiType: Msg.FormEncoded | Msg.UserToken | Msg.EnterpriseGrid,
                      scopes: new[] { "admin.apps:read" } ) ]
    public class AdminAppsRequestList : SlackMessage
    {
        
        public string? cursor { get; set; }

        
        public int? limit { get; set; }

        
        public string? team_id { get; set; }
    }

    public class AdminAppsRequestListResponse : MessageResponse
    {
        public AppRequestedInfo[] app_requests      { get; set; }
        public MetaData           response_metadata { get; set; }
    }

    [ SlackMessage( "admin.apps.restrict",
                      apiType: Msg.UserToken | Msg.EnterpriseGrid,
                      scopes: new[] { "admin.apps:write" } ) ]
    public class AdminAppRestrict : SlackMessage
    {
        
        public string? app_id { get; set; }

        
        public string? request_id { get; set; }

        
        public string? team_id { get; set; }
    }

    [ SlackMessage( "admin.apps.restrict.list",
                      apiType: Msg.FormEncoded | Msg.UserToken | Msg.EnterpriseGrid,
                      scopes: new[] { "admin.apps:read" } ) ]
    public class AdminAppsRestrictList : SlackMessage
    {
        
        public string? cursor { get; set; }

        
        public int? limit { get; set; }

        
        public string? team_id { get; set; }

        
        public string? enterprise_id { get; set; }
    }

    public class AdminAppsRestrictedListResponse : MessageResponse
    {
        public AppDetailedInfo[] app_requests      { get; set; }
        public MetaData          response_metadata { get; set; }
    }

    [ SlackMessage( "admin.conversations.setTeams",
                      apiType: Msg.FormEncoded | Msg.Json | Msg.EnterpriseGrid,
                      scopes: new[] { "admin.conversations:write" } ) ]
    public class AdminConversationsSetTeams : SlackMessage
    {
        
        public string channel_id { get; set; }

        public bool?  org_channel     { get; set; }
        public string target_teams_id { get; set; }
        public string team_id         { get; set; }
    }

}

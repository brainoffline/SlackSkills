// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

namespace SlackSkills.WebApiContracts
{
    [ SlackMessage( "admin.apps.approve",
                      apiType: Msg.FormEncoded | Msg.Json | Msg.UserToken | Msg.EnterpriseGrid,
                      scopes: new[] { "admin.apps:write" } ) ]
    public class AdminAppApprove : SlackMessage
    {
        public string? app_id        { get; set; }
        public string? enterprise_id { get; set; }
        public string? request_id    { get; set; }
        public string? team_id       { get; set; }
    }

    [ SlackMessage( "admin.apps.clearResolution",
                      apiType: Msg.FormEncoded | Msg.Json | Msg.UserToken | Msg.EnterpriseGrid,
                      scopes: new[] { "admin.apps:write" } ) ]
    public class AdminAppClearResolution : SlackMessage
    {
        public string app_id        { get; set; }
        public string? enterprise_id { get; set; }
        public string? team_id       { get; set; }
    }

    [SlackMessage( "admin.apps.approved.list",
                     apiType: Msg.FormEncoded | Msg.Json | Msg.UserToken | Msg.EnterpriseGrid,
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
                      apiType: Msg.FormEncoded | Msg.Json | Msg.UserToken | Msg.EnterpriseGrid,
                      scopes: new[] { "admin.apps:read" } ) ]
    public class AdminAppsRequestList : SlackMessage
    {
        public string? cursor  { get; set; }
        public int?    limit   { get; set; }
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

    [ SlackMessage( "admin.barriers.create",
                      apiType: Msg.FormEncoded | Msg.Json | Msg.GetMethod | Msg.EnterpriseGrid,
                      scopes: new[] { "admin.barriers:write" } ) ]
    public class AdminBarriersCreate : SlackMessage
    {
        public List<string> barriered_from_usergroup_ids { get; set; }
        public string       primary_usergroup_id         { get; set; }
        public     string       restricted_subjects          { get; set; }
    }

    [SlackMessage( "admin.barriers.delete",
                     apiType: Msg.FormEncoded | Msg.Json | Msg.GetMethod | Msg.EnterpriseGrid,
                     scopes: new[] { "admin.barriers:write" })]
    public class AdminBarriersDelete : SlackMessage
    {
        public string barrier_id { get; set; }
    }

    [SlackMessage( "admin.barriers.list",
                     apiType: Msg.FormEncoded | Msg.Json | Msg.GetMethod | Msg.EnterpriseGrid,
                     scopes: new[] { "admin.barriers:read" })]
    public class AdminBarriersList : SlackMessage
    {
        public string? cursor { get; set; }
        public int?    limit  { get; set; }
    }

    [SlackMessage( "admin.barriers.update",
                     apiType: Msg.FormEncoded | Msg.Json | Msg.GetMethod | Msg.EnterpriseGrid,
                     scopes: new[] { "admin.barriers:write" })]
    public class AdminBarriersUpdate : SlackMessage
    {
        public string       barrier_id                   { get; set; }
        public List<string> barriered_from_usergroup_ids { get; set; }
        public string       primary_usergroup_id         { get; set; }
        public string       restricted_subjects          { get; set; }
    }

    public class AdminAppsRestrictedListResponse : MessageResponse
    {
        public AppDetailedInfo[] app_requests      { get; set; }
        public MetaData          response_metadata { get; set; }
    }
}

using System;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

namespace SlackSkills.WebApiContracts
{
    [ SlackMessage( "admin.teams.admins.list",
                      apiType: Msg.FormEncoded | Msg.UserToken | Msg.EnterpriseGrid,
                      scope: "admin.teams:write" ) ]
    public class AdminTeamsAdminsList : SlackMessage
    {
        
        public string team_id { get; set; }

        public string cursor { get; set; }
        public int?   limit  { get; set; }
    }

    public class AdminTeamsAdminsListResponse : MessageResponse
    {
        public string[] admin_ids { get; set; }
    }

    [ SlackMessage( "admin.teams.create",
                      apiType: Msg.UserToken | Msg.EnterpriseGrid,
                      scope: "admin.teams:write" ) ]
    public class AdminTeamCreate : SlackMessage
    {
        
        public string team_domain { get; set; }

        public string team_name        { get; set; }
        public string team_description { get; set; }

        /// <summary>
        /// Possible options are: open, close, invite_only, unlisted
        /// </summary>
        public string team_discoverability { get; set; }
    }

    public class AdminTeamCreateResponse : MessageResponse
    {
        
        public string team { get; set; }
    }

    [ SlackMessage( "admin.teams.list",
                      apiType: Msg.FormEncoded | Msg.UserToken | Msg.EnterpriseGrid,
                      scope: "admin.teams:read" ) ]
    public class AdminTeamsList : SlackMessage
    {
        
        public string team_id { get; set; }

        public string cursor { get; set; }
        public int?   limit  { get; set; }
    }

    public class AdminTeamsListResponse : MessageResponse
    {
        public Team[]   teams             { get; set; }
        public MetaData response_metadata { get; set; }
    }

    [ SlackMessage( "admin.teams.owners.list",
                      apiType: Msg.FormEncoded | Msg.UserToken | Msg.EnterpriseGrid,
                      scope: "admin.teams:read" ) ]
    public class AdminTeamsOwnersList : SlackMessage
    {
        
        public string team_id { get; set; }

        public string cursor { get; set; }
        public int?   limit  { get; set; }
    }

    public class AdminTeamsOwneresListResponse : MessageResponse
    {
        public string[] owner_ids         { get; set; }
        public MetaData response_metadata { get; set; }
    }

    [ SlackMessage( "admin.teams.settings.info",
                      apiType: Msg.FormEncoded | Msg.Json | Msg.UserToken | Msg.EnterpriseGrid,
                      scope: "admin.teams:read" ) ]
    public class AdminTeamsSettingsInfo : SlackMessage
    {
        public string team_id { get; set; }
    }

    public class AdminTeamsSettingsInfoResponse : MessageResponse
    {
        public Team team { get; set; }
    }

    [ SlackMessage( "admin.teams.settings.setDefaultChannels",
                      apiType: Msg.FormEncoded | Msg.UserToken | Msg.EnterpriseGrid,
                      scope: "admin.teams:write" ) ]
    public class AdminTeamsSettingsSetDefaultChannels : SlackMessage
    {
        
        public string channel_id { get; set; }

        
        public string team_id { get; set; }
    }

    [ SlackMessage( "admin.teams.settings.setDescription",
                      apiType: Msg.FormEncoded | Msg.Json | Msg.UserToken | Msg.EnterpriseGrid,
                      scope: "admin.teams:read" ) ]
    public class AdminTeamsSettingsSetDescription : SlackMessage
    {
        
        public string team_id { get; set; }

        
        public string description { get; set; }
    }

    [ SlackMessage( "admin.teams.settings.setDiscoverability",
                      apiType: Msg.FormEncoded | Msg.Json | Msg.UserToken | Msg.EnterpriseGrid,
                      scope: "admin.teams:read" ) ]
    public class AdminTeamsSettingsSetDiscoverabiulity : SlackMessage
    {
        
        public string team_id { get; set; }

        /// <summary>
        ///     possible values (open, invite_only, closed, unlisted)
        /// </summary>
        
        public string discoverability { get; set; }
    }

    [ SlackMessage( "admin.teams.settings.setIcon",
                      apiType: Msg.FormEncoded | Msg.UserToken | Msg.EnterpriseGrid,
                      scope: "admin.teams:write" ) ]
    public class AdminTeamsSettingsSetIcon : SlackMessage
    {
        
        public string team_id { get; set; }

        public string image_url { get; set; }
    }

    [ SlackMessage( "admin.teams.settings.setName",
                      apiType: Msg.FormEncoded | Msg.Json | Msg.UserToken | Msg.EnterpriseGrid,
                      scope: "admin.teams:write" ) ]
    public class AdminTeamsSettingsSetName : SlackMessage
    {
        
        public string team_id { get; set; }

        public string name { get; set; }
    }
}

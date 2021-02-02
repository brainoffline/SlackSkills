using System;
using System.Collections.Generic;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

namespace SlackForDotNet.WebApiContracts
{
    [ SlackMessage( "team.accessLogs",
                      apiType: Msg.FormEncoded | Msg.UserToken ) ]
    public class TeamAccessLogs : SlackMessage
    {
        public int? before { get; set; }
        public int? count  { get; set; }
        public int? page   { get; set; }
    }

    public class TeamAccessLogsResponse : MessageResponse
    {
        public LoginLog[] logs   { get; set; }
        public Paging     paging { get; set; }
    }

    [ SlackMessage( "team.billableInfo",
                      apiType: Msg.FormEncoded | Msg.UserToken ) ]
    public class TeamBillableInfo : SlackMessage
    {
        public string user { get; set; }
    }

    public class TeamBillableInfoResponse : MessageResponse
    {
        public Dictionary< string, UserBillableInfo > billable_info { get; set; }

        public class UserBillableInfo
        {
            public bool? billing_active { get; set; }
        }
    }

    [ SlackMessage( "team.info",
                      apiType: Msg.FormEncoded | Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "team:read" } ) ]
    public class TeamInfo : SlackMessage
    {
        public string team { get; set; }
    }

    public class TeamInfoResponse : MessageResponse
    {
        public Team team { get; set; }
    }

    /// <summary>
    /// https://api.slack.com/methods/team.integrationLogs
    /// </summary>
    [ SlackMessage( "team.integrationLogs",
                      apiType: Msg.FormEncoded | Msg.UserToken ) ]
    public class TeamIntegrationLogs : SlackMessage
    {
        public string app_id      { get; set; }
        public string change_type { get; set; }
        public int?   count       { get; set; }
        public int?   page        { get; set; }
        public string service_id  { get; set; }
        public string user        { get; set; }
    }

    public class TeamIntegrationLogsResponse : MessageResponse
    {
        public IntegrationLog[] logs   { get; set; }
        public Paging           paging { get; set; }
    }

    [ SlackMessage( "team.profile.get",
                      apiType: Msg.FormEncoded | Msg.UserToken,
                      scopes: new[] { "users.profile:read" } ) ]
    public class TeamProfileGet : SlackMessage
    {
        /// <summary>
        /// Possible values: all, visible, hidden
        /// </summary>
        public string visibility { get; set; }
    }

    public class TeamProfileResponse : MessageResponse
    {
        public ProfileField[] fields { get; set; }

        public class ProfileField
        {
            public string id       { get; set; }
            public int?   ordering { get; set; }
            public string label    { get; set; }
            public string hint     { get; set; }

            public string type { get; set; }

            //public string possible_values { get; set; }
            public ProfileOption options { get; set; }
        }

        public class ProfileOption
        {
            public int? is_protected { get; set; }
        }
    }
}

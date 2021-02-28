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
    [SlackMessage( "apps.connections.open",
                     apiType: Msg.Json | Msg.FormEncoded | Msg.AppLevel,
                     scope: "connections:write")]
    public class AppsConnectionsOpenRequest : SlackMessage
    {
    }

    public class AppsConnectionsOpenResponse : MessageResponse
    {
        public string url { get; set; }
    }


    [SlackMessage( "apps.event.authorizations.list",
                     apiType: Msg.Json | Msg.FormEncoded | Msg.AppLevel,
                     scope: "authorizations:read")]
    public class AppsEventAuthorizationsListRequest : SlackMessage
    {
        public string event_contest { get; set; }
        public string cursor        { get; set; }
        public int?   limit         { get; set; }
    }

    public class AppsEventAuthorizationsList : MessageResponse
    {
        public List<Authorization>? authroizations { get; set; }
    }

    [ SlackMessage( "apps.uninstall",
                      apiType: Msg.Json | Msg.FormEncoded | Msg.BotToken,
                      scope: "" ) ]
    public class AppsUninstall : SlackMessage
    {
        public string client_id     { get; set; }
        public string client_secret { get; set; }
    }

}

using System;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

namespace SlackSkills.WebApiContracts
{
    [ SlackMessage( "bots.info",
                      apiType: Msg.FormEncoded | Msg.BotToken | Msg.UserToken,
                      scope: "users:read" ) ]
    public class BotInfoRequest : SlackMessage
    {
        public string? bot     { get; set; }
        public string? team_id { get; set; }
    }

    public class BotInfoResponse : MessageResponse
    {
        public Bot bot { get; set; }
    }
}

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
    [ SlackMessage( "dialog.open",
                      apiType: Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "bot" } ) ]
    public class DialogOpen : SlackMessage
    {
        public Dialog dialog { get; set; }
        public string trigger_id { get; set; }
    }
}

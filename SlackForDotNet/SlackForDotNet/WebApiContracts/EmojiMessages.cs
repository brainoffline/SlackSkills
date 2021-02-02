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
    [ SlackMessage( "emoji.list",
                      apiType: Msg.FormEncoded | Msg.BotToken | Msg.UserToken,
                      scope: "emoji:read" ) ]
    public class EmojiList : SlackMessage { }

    public class EmojiListResponse : MessageResponse
    {
        public Dictionary< string, string > emoji { get; set; }
    }
}

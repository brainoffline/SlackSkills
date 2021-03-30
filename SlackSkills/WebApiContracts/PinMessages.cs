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
    [ SlackMessage( "pins.add",
                      apiType: Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "pins:write" } ) ]
    public class PinAdd : SlackMessage
    {
        
        public string channel { get; set; }

        
        public string timestamp { get; set; }
    }

    [ SlackMessage( "pins.list",
                      apiType: Msg.FormEncoded | Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "pins:read" } ) ]
    public class PinsList : SlackMessage
    {
        public string channel { get; set; }
    }

    [ SlackMessage( "pins.remove",
                      apiType: Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "pins:write" } ) ]
    public class PinRemove : SlackMessage
    {
        
        public string channel { get; set; }

        
        public string timestamp { get; set; }
    }

    public class PinsListResponse : MessageResponse
    {
        public Item[] items { get; set; }
    }
}

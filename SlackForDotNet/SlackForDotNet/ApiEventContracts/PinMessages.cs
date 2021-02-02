using System;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

namespace SlackForDotNet
{
    [ SlackMessage( "pin_added", apiType: Msg.RTM | Msg.Event, scope: "pin:read" ) ]
    public class PinAdded : SlackMessage
    {
        public string      channel_id { get; set; }
        public MessageBase item       { get; set; }
    }

    [ SlackMessage( "pin_removed", apiType: Msg.RTM | Msg.Event, scope: "pin:read" ) ]
    public class PinRemoved : SlackMessage
    {
        public string      channel_id { get; set; }
        public MessageBase item       { get; set; }
        public bool?       has_pins   { get; set; }
    }
}

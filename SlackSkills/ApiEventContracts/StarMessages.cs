using System;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

namespace SlackSkills
{
    [ SlackMessage( "star_added", apiType: Msg.RTM | Msg.Event, scope: "stars:read" ) ]
    public class StarAdded : SlackMessage
    {
        public string      user     { get; set; }
        public MessageBase item     { get; set; }
    }

    [ SlackMessage( "star_removed", apiType: Msg.RTM | Msg.Event, scope: "stars:read" ) ]
    public class StarRemoved : SlackMessage
    {
        public string      user     { get; set; }
        public MessageBase item     { get; set; }
    }
}

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
    [ SlackMessage( "reaction_added", apiType: Msg.RTM | Msg.Event, scope: "reactions:read" ) ]
    public class ReactionAdded : SlackMessage
    {
        public string      user      { get; set; }
        public string      reaction  { get; set; }
        public string      item_user { get; set; }
        public MessageBase item      { get; set; }
    }

    [ SlackMessage( "reaction_removed", apiType: Msg.RTM | Msg.Event, scope: "reactions:read" ) ]
    public class ReactionRemoved : SlackMessage
    {
        public string      user      { get; set; }
        public string      reaction  { get; set; }
        public string      item_user { get; set; }
        public MessageBase item      { get; set; }
    }
}

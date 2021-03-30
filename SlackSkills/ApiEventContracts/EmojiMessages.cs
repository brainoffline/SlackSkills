// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

using System;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

namespace SlackSkills
{
    [ SlackMessage( "emoji_changed", subType: "add", apiType: Msg.RTM | Msg.Event, scope: "emoji:read" ) ]
    public class EmojiAddedMessage : SlackMessage
    {
        public string name     { get; set; }
        public string value    { get; set; }
    }

    [SlackMessage( "emoji_changed", subType: "remove", apiType: Msg.RTM | Msg.Event, scope: "emoji:read")]
    public class EmojiRemovedMessage : SlackMessage
    {
        public string[] names { get; set; }
    }

    [SlackMessage( "emoji_changed", subType: "rename", apiType: Msg.RTM | Msg.Event, scope: "emoji:read")]
    public class EmojiRenamedMessage : SlackMessage
    {
        public string old_name { get; set; }
        public string new_name { get; set; }
        public string value    { get; set; }
        public string event_ts { get; set; }
    }
}

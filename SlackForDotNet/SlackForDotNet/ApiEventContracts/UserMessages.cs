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
    [ SlackMessage( "user_change", scope: "users:read" ) ]
    public class UserChange : SlackMessage
    {
        public User user { get; set; }
    }

    [ SlackMessage( "user_typing" ) ]
    public class UserTyping : SlackMessage
    {
        public string channel { get; set; }
        public string user    { get; set; }
    }
}

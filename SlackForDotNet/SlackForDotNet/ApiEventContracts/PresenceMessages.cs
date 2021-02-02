// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

using System;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

namespace SlackForDotNet
{
    [ SlackMessage( "manual_presence_change" ) ]
    public class ManualPresenceChange : SlackMessage
    {
        public string user     { get; set; }
        public string presence { get; set; }
    }

    [ SlackMessage( "presence_change" ) ]
    public class PresenceChange : SlackMessage
    {
        public string   user     { get; set; }
        public string[] users    { get; set; }
        public string   presence { get; set; }
    }

    [ SlackMessage( "presence_query" ) ]
    public class PresenceQuery : SlackMessage
    {
        public string[] ids { get; set; }
    }

    [ SlackMessage( "presence_sub" ) ]
    public class PresenceSub : SlackMessage
    {
        public string[] ids { get; set; }
    }
}

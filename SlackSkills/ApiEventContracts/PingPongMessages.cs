// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

using System;

namespace SlackSkills
{
    [ SlackMessage( "ping" ) ]
    public class Ping : SlackMessage { }

    [ SlackMessage( "pong" ) ]
    public class Pong : SlackMessage { }
}

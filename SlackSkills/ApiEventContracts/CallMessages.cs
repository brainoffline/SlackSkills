// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

using System;

namespace SlackSkills
{
    [ SlackMessage( "call_rejected", apiType: Msg.Event, scope: "calls:read" ) ]
    public class CallRejected : SlackMessage { }
}

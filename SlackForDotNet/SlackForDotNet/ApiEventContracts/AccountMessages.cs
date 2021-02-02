// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

using System;

namespace SlackForDotNet
{
    [ SlackMessage( "accounts_changed", apiType: Msg.RTM ) ]
    public class AccountsChanged : SlackMessage { }
}

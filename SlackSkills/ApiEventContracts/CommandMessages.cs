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
    [ SlackMessage( "commands_changed" ) ]
    public class CommandsChanged : SlackMessage
    {
        public string[]? commands_updated { get; set; }
        public string[]? commands_removed { get; set; }
    }

}

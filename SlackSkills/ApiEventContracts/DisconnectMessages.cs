#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

namespace SlackSkills
{
    [SlackMessage( "disconnect")]
    public class Disconnect : SlackMessage
    {
        public string?    reason     { get; set; }
        public DebugInfo? debug_info { get; set; }
    }

}

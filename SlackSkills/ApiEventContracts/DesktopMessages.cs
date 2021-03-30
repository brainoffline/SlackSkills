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
    [ SlackMessage( "desktop_notification" ) ]
    public class DesktopNotification : SlackMessage
    {
        public string title             { get; set; }
        public string subtitle          { get; set; }
        public string msg               { get; set; }
        public string ts                { get; set; }
        public string content           { get; set; }
        public string channel           { get; set; }
        public string launchuri         { get; set; }
        public string avatarImage       { get; set; }
        public string ssbFilename       { get; set; }
        public string imageUri          { get; set; }
        public bool   is_shared         { get; set; }
        public bool   is_channel_invite { get; set; }
    }
}

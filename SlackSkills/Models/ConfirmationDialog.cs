using System;

using SlackSkills.Surface;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

namespace SlackSkills
{
    public class ConfirmationDialog
    {
        public PlainText title   { get; set; }
        public Text      text    { get; set; }
        public PlainText confirm { get; set; }
        public PlainText deny    { get; set; }
        public string         style   { get; set; }
    }

}

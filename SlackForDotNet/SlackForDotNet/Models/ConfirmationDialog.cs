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
    public class ConfirmationDialog
    {
        public PlainTextBlock title   { get; set; }
        public TextBlock      text    { get; set; }
        public PlainTextBlock confirm { get; set; }
        public PlainTextBlock deny    { get; set; }
        public string         style   { get; set; }
    }
}

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
    [Obsolete("")]
    public class Dialog
    {
        public string callback_id      { get; set; }
        public string title            { get; set; }
        public string submit_label     { get; set; }
        public bool?  notify_on_cancel { get; set; }
        public string state            { get; set; }
        public Dialog dialog           { get; set; }
    }
}

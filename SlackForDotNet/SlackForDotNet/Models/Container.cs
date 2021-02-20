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
    public class Container
    {
        public string  type          { get; set; }
        public string? message_ts    { get; set; }
        public int?    attachment_id { get; set; }
        public string  channel_id    { get; set; }
        public string? message       { get; set; }
        public string? text          { get; set; }
        public bool?   is_ephemeral  { get; set; }
        public bool?   is_app_unfurl { get; set; }
        public string? view_id       { get; set; }
    }
}

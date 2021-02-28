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
    public abstract class MessageBase : SlackMessage
    {
        public string  ts           { get; set; }
        public string? text         { get; set; }
        public string? user         { get; set; }
        public string? team         { get; set; }
        public string  channel      { get; set; }
        public string? channel_type { get; set; }
        public bool?   hidden       { get; set; }
    }
}

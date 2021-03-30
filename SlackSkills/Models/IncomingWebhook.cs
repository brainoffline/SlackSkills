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
    public class IncomingWebhook
    {
        public string channel           { get; set; }
        public string channel_id        { get; set; }
        public string configuration_url { get; set; }
        public string url               { get; set; }
    }
}

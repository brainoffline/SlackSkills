using System;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

namespace SlackSkills
{
    public class ResponseUrl
    {
        public string block_id     { get; set; }
        public string action_id    { get; set; }
        public string channel_id   { get; set; }
        public string response_url { get; set; }

    }
}

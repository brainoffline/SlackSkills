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
    public class InviteRequest
    {
        public string   id             { get; set; }
        public string   email          { get; set; }
        public int?     date_created   { get; set; }
        public string[] requester_ids  { get; set; }
        public string   channel_ids    { get; set; }
        public string   real_name      { get; set; }
        public int      date_expire    { get; set; }
        public string   request_reason { get; set; }
        public Team     team           { get; set; }
    }
}

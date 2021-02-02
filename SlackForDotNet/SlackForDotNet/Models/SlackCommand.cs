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
    public class SlackCommand
    {
        public string token        { get; set; }
        public string team_id      { get; set; }
        public string team_domain  { get; set; }
        public string channel_id   { get; set; }
        public string channel_name { get; set; }
        public string user_id      { get; set; }
        public string user_name    { get; set; }
        public string command      { get; set; }
        public string text         { get; set; }
        public string response_url { get; set; }
        public string trigger_id   { get; set; }
    }
}

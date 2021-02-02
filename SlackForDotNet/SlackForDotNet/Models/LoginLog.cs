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
    public class LoginLog
    {
        public string user_id    { get; set; }
        public string username   { get; set; }
        public int?   date_first { get; set; }
        public int?   date_last  { get; set; }
        public int?   count      { get; set; }
        public string ip         { get; set; }
        public string user_agent { get; set; }
        public string isp        { get; set; }
        public string country    { get; set; }
        public string region     { get; set; }
    }
}

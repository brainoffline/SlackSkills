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
    public class Session
    {
        public string user_id    { get; set; }
        public string team_id    { get; set; }
        public int    session_id { get; set; }
    }

    public class SessionInfo
    {
        public string device_hardware      { get; set; }
        public string os                   { get; set; }
        public string os_version           { get; set; }
        public string slack_client_version { get; set; }
        public string ip                   { get; set; }
    }
}

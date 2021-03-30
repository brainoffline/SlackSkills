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
    public class IntegrationLog
    {
        public string app_id       { get; set; }
        public string app_type     { get; set; }
        public string service_id   { get; set; }
        public string service_type { get; set; }
        public string user_id      { get; set; }
        public string user_name    { get; set; }
        public string channel      { get; set; }
        public string date         { get; set; }

        /// <summary>
        /// Possible values: added, removed, enabled, disabled, updated
        /// </summary>
        public string change_type { get; set; }

        public string reason { get; set; }
        public string scope  { get; set; }
    }
}

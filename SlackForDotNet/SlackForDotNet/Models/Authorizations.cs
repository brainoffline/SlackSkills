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
    public class Authorization
    {
        public string enterprise_id         { get; set; }
        public string team_id               { get; set; }
        public string user_id               { get; set; }
        public bool?  is_bot                { get; set; }
        public bool?  is_enterprise_install { get; set; }
    }
}

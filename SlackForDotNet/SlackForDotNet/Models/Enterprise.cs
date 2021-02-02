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
    public class Enterprise
    {
        public string? id { get; set; }
        public string? name { get; set; }

        public string?  domain          { get; set; }
        public string?  enterprise_id   { get; set; }
        public string?  enterprise_name { get; set; }
        public string[] teams           { get; set; }
        public bool?    is_admin        { get; set; }
        public bool?    is_owner        { get; set; }
    }
}

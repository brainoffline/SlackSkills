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
    public class Scopes
    {
        public string name         { get; set; }
        public string description  { get; set; }
        public bool?  is_sensitive { get; set; }

        /// <summary>
        /// Possible values: app, bot, user
        /// </summary>
        public string token_type { get; set; }
    }
}

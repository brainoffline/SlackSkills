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
    public class Resolution
    {
        /// <summary>
        /// Possible Values: approved, restricted
        /// </summary>
        public string status { get; set; }

        public Scopes[] scopes { get; set; }
    }
}

﻿// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

using System;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

namespace SlackSkills
{
    public class InstantMessage
    {
        public string id              { get; set; }
        public bool?  is_im           { get; set; }
        public string user            { get; set; }
        public int?   created         { get; set; }
        public bool?  is_user_deleted { get; set; }
    }
}

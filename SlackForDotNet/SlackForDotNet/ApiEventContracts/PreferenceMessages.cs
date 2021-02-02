﻿// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

using System;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

namespace SlackForDotNet
{
    [ SlackMessage( "pref_change" ) ]
    public class PreferenceChange : SlackMessage
    {
        public string name  { get; set; }
        public string value { get; set; }
    }
}

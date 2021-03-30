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
    [ SlackMessage( "tokens_revoked", apiType: Msg.Event ) ]
    public class TokensRevoked : SlackMessage
    {
        public Tokens tokens { get; set; }

        public class Tokens
        {
            public string[] oauth { get; set; }
            public string[] bot   { get; set; }
        }
    }
}

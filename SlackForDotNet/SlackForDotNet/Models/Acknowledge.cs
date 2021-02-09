#pragma warning disable 8618

using System;
using System.Collections.Generic;

using SlackForDotNet.Surface;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

namespace SlackForDotNet
{
    public record Acknowledge(string envelope_id);

    public record AcknowledgeOptions( string envelope_id, List< Option > options );
}

#pragma warning disable 8618

using System;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

namespace SlackForDotNet
{
    public record Acknowledge(string envelope_id);

    public class Acknowledge<TPayload> where TPayload : class
    {
        public string   envelope_id { get; set; }
        public TPayload payload     { get; set; }
    }
}

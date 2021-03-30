#pragma warning disable 8618

using System;

using JetBrains.Annotations;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

namespace SlackSkills
{
    public record Acknowledge(string envelope_id);

    public class AcknowledgeResponse<T>
    {
        public string envelope_id { get; set; }
        public T      payload     { get; set; }

        public AcknowledgeResponse() { }
        public AcknowledgeResponse(string envelopeId, T payload)
        {
            envelope_id  = envelopeId;
            this.payload = payload;
        }
    }
}

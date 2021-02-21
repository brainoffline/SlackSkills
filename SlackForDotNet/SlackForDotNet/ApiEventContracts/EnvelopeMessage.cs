#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

using System;

namespace SlackForDotNet
{
    public abstract class Envelope : SlackMessage
    {
        public          string      envelope_id              { get; set; }
        public          bool        accepts_response_payload { get; set; }
        public          int         retry_attempt            { get; set; }
        public          string?     retry_reason             { get; set; }
    }
    
    public interface IEnvelope<out T> where T : SlackMessage
    {
        public string envelope_id              { get; }
        public bool   accepts_response_payload { get; }
        T?            payload                  { get; }
    }

    public class Envelope<T> : Envelope, IEnvelope<T> where T : SlackMessage
    {
        public T? payload { get; set; }
    }
}

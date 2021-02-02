// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

using System.Reflection;
using System.Runtime.CompilerServices;

using Newtonsoft.Json;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

namespace SlackForDotNet
{
    [JsonConverter(typeof(SlackMessageConverter))]
    public class SlackMessage
    {
        public SlackMessage()
        {
            var messageType = GetType().GetTypeInfo().GetCustomAttribute<SlackMessageAttribute>();
            if (messageType == null)
                return;
            type    = messageType.Type;
            subtype = messageType.SubType;
        }

        public string  type    { get; set; }
        public string? subtype { get; set; }
        public int?    id      { get; set; }
    }

    public class MessageResponse : SlackMessage
    {
        public bool?   ok             { get; set; }
        public bool?   no_op          { get; set; }
        public bool?   not_in_channel { get; set; }
        public string? error          { get; set; }
        public string? warning        { get; set; }
        public string? detail         { get; set; }
        public string? provided       { get; set; }
        public string? needed         { get; set; }
    }
}

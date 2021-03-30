using System;
using System.Collections.Generic;

namespace SlackSkills 
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class SlackMessageAttribute : Attribute
    {
        public string       Type    { get; }
        public string?      SubType { get; }
        public Msg          ApiType { get; }
        public List<string> Scopes  { get; } = new();

        public SlackMessageAttribute(
            string    type, 
            string?   subType = null, 
            Msg       apiType = Msg.Json,
            string[]? scopes  = null,
            string?   scope   = null)
        {
            Type = type;
            SubType = subType;
            ApiType = apiType;
            if (scopes != null) Scopes.AddRange(scopes);
            if (scope != null)  Scopes.Add(scope);
        }
    }

    [Flags]
    public enum Msg
    {
        Json = 0x01,
        FormEncoded = 0x02,
        Multipart = 0x04,
        RTM = 0x08 | Json,
        Event = 0x10 | Json,
        UserToken = 0x20,
        BotToken = 0x40,
        EnterpriseGrid = 0x80,
        AppLevel = 0x100,
        GetMethod = 0x200
    }
}

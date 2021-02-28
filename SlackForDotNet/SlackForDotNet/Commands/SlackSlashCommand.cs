using System;
using System.Collections.Generic;

using Asciis;

namespace SlackForDotNet
{
    public abstract class SlackSlashCommand 
    {
        public ISlackApp?               SlackApp { get; set; }
        public SlashCommand?            Message  { get; set; }
        public IEnvelope<SlackMessage>? Envelope { get; set; }

        public abstract void OnCommand(List<string> args);

        [Action( "Execute", "Default action", DefaultAction = true)]
        public void Execute(List<string> args)
        {
            OnCommand(args);
        }
    }

    /// <summary>
    /// Add Command metadata
    /// </summary>
    [AttributeUsage( AttributeTargets.Class)]
    public sealed class SlashCommandAttribute : Attribute
    {
        public SlashCommandAttribute(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Invalid name", nameof(name));
            Name = !name.StartsWith("/")
                       ? "/" + name
                       : name;
        }

        public string  Name        { get; set; }
        public string? Description { get; set; }
    }

}

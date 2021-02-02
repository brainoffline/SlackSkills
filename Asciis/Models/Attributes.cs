using System;
using System.Collections.Generic;
using System.Linq;

namespace Asciis
{
    /// <summary>
    /// Add Command metadata
    /// </summary>
    [AttributeUsage( AttributeTargets.Class)]
    public sealed class CommandAttribute : Attribute
    {
        public CommandAttribute(string name)
        {
            Names = name.Split('|')
                        .ToList();
        }

        public List<string> Names       { get; set; }
        public string?      Pattern     { get; set; }
        public string?      Description { get; set; }
    }

    /// <summary>
    /// Add to a property for passing parameters
    /// </summary>
    [AttributeUsage( AttributeTargets.Property)]
    public sealed class ParamAttribute : Attribute
    {
        public ParamAttribute(string pattern, string description)
        {
            Pattern     = pattern     ?? throw new ArgumentNullException(nameof(pattern));
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }
        public string Pattern     { get; set; }
        public string Description { get; set; }
    }
    
    /// <summary>
    /// Execute the method when the action matches
    /// </summary>
    [AttributeUsage( AttributeTargets.Method)]
    public sealed class ActionAttribute : Attribute
    {
        public ActionAttribute(string pattern, string description)
        {
            Pattern     = pattern;
            Description = description;
        }

        public string? Pattern       { get; set; }
        public string? Description   { get; set; }
        public bool   DefaultAction { get; set; }
    }

}

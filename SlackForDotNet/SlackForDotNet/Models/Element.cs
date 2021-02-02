// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

using System;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

namespace SlackForDotNet
{
    public class Element
    {
        public string type     { get; set; }
        public string sub_type { get; set; }
    }

    [ SlackElement( "text" ) ]
    [ SlackElement( "text", "email" ) ]
    [ SlackElement( "text", "number" ) ]
    [ SlackElement( "text", "tel" ) ]
    [ SlackElement( "text", "url" ) ]
    [ SlackElement( "textarea" ) ]
    [ SlackElement( "textarea", "email" ) ]
    [ SlackElement( "textarea", "number" ) ]
    [ SlackElement( "textarea", "tel" ) ]
    [ SlackElement( "textarea", "url" ) ]
    public class TextElement : Element
    {
        public string label       { get; set; }
        public string name        { get; set; }
        public string placeholder { get; set; }
        public string value       { get; set; }

        public int?   min_length { get; set; }
        public int?   max_length { get; set; }
        public bool?  optional   { get; set; }
        public string hint       { get; set; }
    }

    [ SlackElement( "select" ) ]
    public class SelectElement : Element
    {
        public string label       { get; set; }
        public string name        { get; set; }
        public string placeholder { get; set; }
        public string value       { get; set; }

        public ElementOption[]      selected_options { get; set; }
        public ElementOption[]      options          { get; set; }
        public ElementOptionGroup[] option_groups    { get; set; }

        public bool? optional { get; set; }

        public int? min_query_length { get; set; }

        /// <summary>
        /// Possible Values: users, channels, conversations, external
        /// </summary>
        public string data_source { get; set; }
    }

    public class ElementOption
    {
        public string label { get; set; }
        public string value { get; set; }
    }

    public class ElementOptionGroup
    {
        public string          label   { get; set; }
        public ElementOption[] options { get; set; }
    }
}

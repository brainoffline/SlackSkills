using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

namespace SlackForDotNet
{
    [JsonConverter( typeof(SlackBlockConverter))]
    public class Block
    { 
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private static int idSource;

        public Block()
        {
            var messageType = GetType().GetTypeInfo().GetCustomAttribute<SlackBlockAttribute>();
            if (messageType == null)
                return;
            type     = messageType.Type;
            if (messageType.BlockIdRequired == true)
                block_id = $"{type}-{Interlocked.Increment(ref idSource)}";
        }

        public string type     { get; set; }
        public string? block_id { get; set; }
    }

    /// <summary>
    /// https://api.slack.com/reference/block-kit/blocks#section
    /// </summary>
    [ SlackBlock( "section", blockIdRequired: true ) ]
    public class SectionBlock : Block
    {
        public TextBlock     text      { get; set; }
        public TextBlock[]?  fields    { get; set; }
        public ElementBlock? accessory { get; set; }
    }

    /// <summary>
    /// https://api.slack.com/reference/block-kit/blocks#divider
    /// </summary>
    [ SlackBlock( "divider" ) ]
    public class DividerBlock : Block { }

    /// <summary>
    /// https://api.slack.com/reference/block-kit/blocks#image
    /// </summary>
    [ SlackBlock( "image" ) ]
    public class ImageBlock : Block
    {
        
        public string image_url { get; set; }

        
        public string alt_text { get; set; }

        public TextBlock? title { get; set; }
    }

    [ SlackBlock( "actions" ) ]
    public class ActionBlock : ElementsBlock { }

    [ SlackBlock( "context" ) ]
    public class ContextBlock : ElementsBlock { }

    [ SlackBlock( "input" ) ]
    public class InputBlock : Block
    {
        
        public TextBlock label { get; set; }

        
        public ElementBlock? element { get; set; }

        public TextBlock? hint { get; set; }

        public bool? optional { get; set; }
    }

    [ SlackBlock( "file" ) ]
    public class FileBlock : Block
    {
        
        public string external_id { get; set; }

        
        public string source { get; set; } // always `remote`
    }

    public class ElementBlock : Block { }

    [ SlackBlock( "text" ) ]
    public class TextBlock : ElementBlock
    {
        public static implicit operator TextBlock( string text ) => new MarkdownTextBlock( text );

        public static readonly TextBlock Empty = new() { text = string.Empty };

        
        public string text { get; set; }

        public bool? emoji { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary< string, bool >? style { get; set; }
    }

    [ SlackBlock( "plain_text" ) ]
    public class PlainTextBlock : TextBlock
    {
        public static implicit operator PlainTextBlock( string str ) => new() { text = str, emoji = true };
    }

    [ SlackBlock( "emoji" ) ]
    public class EmojiBlock : ElementBlock
    {
        public static implicit operator EmojiBlock( string name ) => new() { name = name };

        
        public string name { get; set; }
    }

    [ SlackBlock( "mrkdwn" ) ]
    public class MarkdownTextBlock : TextBlock
    {
        public MarkdownTextBlock() { }

        public MarkdownTextBlock( string text )
        {
            type      = "mrkdwn";
            this.text = text;
        }
    }

    [ SlackElement( "button" ) ]
    public class ButtonElement : ElementBlock
    {
        
        public TextBlock text { get; set; }

        
        public string action_id { get; set; }

        public string? url { get; set; }

        public string? value { get; set; }

        /// <summary>
        /// Can be `primary`, `danger`, null
        /// </summary>
        
        public string? style { get; set; }

        /// <summary>A confirm object that defines an optional confirmation dialog after the button is clicked. </summary>
        
        public Block? confirm { get; set; }
    }

    [ SlackElement( "datepicker" ) ]
    public class DatePickerElement : ElementBlock
    {
        
        public string action_id { get; set; }

        
        public PlainTextBlock? placeholder { get; set; }

        
        public string? initial_date { get; set; }

        /// <summary>
        /// A confirm object that defines an optional confirmation dialog that appears after a date is selected.
        /// </summary>
        public Block? confirm { get; set; }
    }

    [ SlackElement( "image" ) ]
    public class ImageElement : ElementBlock
    {
        
        public string image_url { get; set; }

        
        public string alt_text { get; set; }

        
        public TextBlock? title { get; set; }
    }

    [ SlackElement( "multi_static_select" ) ]
    public class MultiStaticSelectElement : ElementBlock
    {
        
        public PlainTextBlock placeholder { get; set; }

        
        public string action_id { get; set; }

        
        public BlockOption[] options { get; set; }

        public OptionGroup[]? option_groups { get; set; }

        public BlockOption[]? initial_options { get; set; }

        /// <summary>
        /// A confirm object that defines an optional confirmation dialog that appears before the multi-select choices are submitted.
        /// </summary>
        public Block? confirm { get; set; }
    }

    [ SlackElement( "multi_external_select" ) ]
    public class MultiExternalSelectElement : ElementBlock
    {
        
        public PlainTextBlock placeholder { get; set; }

        
        public string action_id { get; set; }

        
        public int? min_query_length { get; set; }

        
        public BlockOption[]? initial_options { get; set; }

        /// <summary>
        /// A confirm object that defines an optional confirmation dialog that appears before the multi-select choices are submitted.
        /// </summary>
        
        public Block? confirm { get; set; }
    }

    [ SlackElement( "multi_users_select" ) ]
    public class MultiUsersSelectElement : ElementBlock
    {
        
        public PlainTextBlock placeholder { get; set; }

        
        public string action_id { get; set; }

        
        public string[]? initial_users { get; set; }

        /// <summary>
        /// A confirm object that defines an optional confirmation dialog that appears before the multi-select choices are submitted.
        /// </summary>
        
        public Block? confirm { get; set; }
    }

    [ SlackElement( "multi_conversations_select" ) ]
    public class MultiConversationsSelectElement : ElementBlock
    {
        
        public PlainTextBlock placeholder { get; set; }

        
        public string action_id { get; set; }

        
        public string[]? initial_conversations { get; set; }

        /// <summary>
        /// A confirm object that defines an optional confirmation dialog that appears before the multi-select choices are submitted.
        /// </summary>
        
        public Block? confirm { get; set; }

        public int?   max_selected_items { get; set; }
        public Filter filter             { get; set; }
    }

    [ SlackElement( "multi_channels_select" ) ]
    public class MultiChannelsSelectElement : ElementBlock
    {
        
        public PlainTextBlock placeholder { get; set; }

        
        public string action_id { get; set; }

        
        public string[]? initial_channels { get; set; }

        /// <summary>
        /// A confirm object that defines an optional confirmation dialog that appears before the multi-select choices are submitted.
        /// </summary>
        
        public Block? confirm { get; set; }
    }

    [ SlackElement( "overflow" ) ]
    public class OverflowElement : ElementBlock
    {
        
        public string action_id { get; set; }

        
        public BlockOption[]? options { get; set; }

        /// <summary>
        /// A confirm object that defines an optional confirmation dialog that appears before the multi-select choices are submitted.
        /// </summary>
        
        public Block? confirm { get; set; }
    }

    [ SlackElement( "plain_text_input" ) ]
    public class PlainTextInputElement : ElementBlock
    {
        
        public string action_id { get; set; }

        
        public PlainTextBlock? placeholder { get; set; }

        
        public string? initial_value { get; set; }

        
        public bool? multiline { get; set; }

        
        public int? min_length { get; set; }

        
        public int? max_length { get; set; }
    }

    [ SlackElement( "radio_buttons" ) ]
    public class RadioButtonsElement : ElementBlock
    {
        
        public string action_id { get; set; }

        
        public BlockOption[] options { get; set; }

        
        public BlockOption? InitialBlockOption { get; set; }

        /// <summary>
        /// A confirm object that defines an optional confirmation dialog that appears after clicking one of the radio buttons in this element.
        /// </summary>
        
        public Block? confirm { get; set; }
    }

    [ SlackElement( "user" ) ]
    public class UserElement : ElementBlock
    {
        public string user_id { get; set; }
    }

    public class ElementsBlock : ElementBlock
    {
        public List< ElementBlock > elements { get; set; } = new();

        public ElementsBlock Add( ElementBlock element ) 
        {
            elements.Add( element );

            return this;
        }
    }

    [ SlackBlock( "rich_text" ) ]
    public class RichTextBlock : ElementsBlock { }

    [ SlackBlock( "rich_text_list" ) ]
    public class RichTextListBlock : ElementsBlock
    {
        
        public string? style { get; set; }

        public int? indent { get; set; }
    }

    [ SlackBlock( "rich_text_preformatted", blockIdRequired: true) ]
    public class RichTextPreformattedBlock : ElementsBlock { }

    [ SlackBlock( "rich_text_quote", blockIdRequired: true) ]
    public class RichTextQuoteBlock : ElementsBlock { }

    [ SlackBlock( "rich_text_section", blockIdRequired: true ) ]
    public class RichTextSectionBlock : ElementsBlock { }

    [ SlackElement( "static_select", blockIdRequired: true) ]
    public class SelectMenuStaticElement : ElementBlock
    {
        
        public PlainTextBlock placeholder { get; set; }

        
        public string action_id { get; set; }

        
        public BlockOption[] options { get; set; }

        public OptionGroup[]? option_groups { get; set; }

        
        public BlockOption? InitialBlockOption { get; set; }

        /// <summary>
        /// A confirm object that defines an optional confirmation dialog that appears after a menu item is selected.
        /// </summary>
        
        public Block? confirm { get; set; }
    }

    [ SlackElement( "external_select", blockIdRequired: true) ]
    public class SelectMenuExternalElement : ElementBlock
    {
        
        public PlainTextBlock placeholder { get; set; }

        
        public string action_id { get; set; }

        
        public BlockOption? InitialBlockOption { get; set; }

        
        public int? min_query_length { get; set; }

        /// <summary>
        /// A confirm object that defines an optional confirmation dialog that appears after a menu item is selected.
        /// </summary>
        
        public Block? confirm { get; set; }
    }

    [ SlackElement( "users_select", blockIdRequired: true) ]
    public class UsersSelectElement : ElementBlock
    {
        
        public PlainTextBlock placeholder { get; set; }

        
        public string action_id { get; set; }

        
        public string? initial_user { get; set; }

        /// <summary>
        /// A confirm object that defines an optional confirmation dialog that appears after a menu item is selected.
        /// </summary>
        
        public Block? confirm { get; set; }
    }

    [ SlackElement( "conversations_select", blockIdRequired: true) ]
    public class ConversationsSelectElement : ElementBlock
    {
        
        public PlainTextBlock placeholder { get; set; }

        
        public string action_id { get; set; }

        
        public string? initial_conversation { get; set; }

        /// <summary>
        /// A confirm object that defines an optional confirmation dialog that appears after a menu item is selected.
        /// </summary>
        
        public Block? confirm { get; set; }

        public Filter filter { get; set; }
    }

    [ SlackElement( "channel_select", blockIdRequired: true) ]
    public class ChannelSelectElement : ElementBlock
    {
        
        public PlainTextBlock placeholder { get; set; }

        
        public string action_id { get; set; }

        
        public string? initial_channel { get; set; }

        /// <summary>
        /// A confirm object that defines an optional confirmation dialog that appears after a menu item is selected.
        /// </summary>
        
        public Block? confirm { get; set; }
    }

    public class BlockOption
    {
        
        public TextBlock text { get; set; }

        
        public string value { get; set; }

        
        public string? url { get; set; }
    }

    public class OptionGroup
    {
        
        public PlainTextBlock label { get; set; }

        
        public BlockOption[] options { get; set; }
    }
}

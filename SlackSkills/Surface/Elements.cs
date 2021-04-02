using System;
using System.Collections.Generic;
using System.Globalization;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// ReSharper disable InconsistentNaming

#pragma warning disable 8618

namespace SlackSkills.Surface
{
    public delegate Options SuggestionsHandler(string value);

    [JsonConverter( typeof(ElementConverter))]
    public interface IElement
    {
        string  type      { get; }
        string? action_id { get; set; }
    }

    //[JsonConverter( typeof(ElementConverter))]
    public abstract class Element : IElement
    {
        public string  type      { get; }
        public string? action_id { get; set; }

        protected Element( string type, string? actionId )
        {
            this.type = type;
            action_id = actionId;
        }
    }

    public interface IContextLayoutElement : IElement { }
    public interface IInputLayoutElement : IElement { }
    public interface ISectionLayoutElement : IElement { }
    public interface IActionsLayoutElement : IElement { }

    /// <summary>
    /// Display Text
    /// </summary>
    public class PlainTextElement : Element,
                                    IContextLayoutElement,
                                    ISectionLayoutElement,
                                    IActionsLayoutElement,
                                    IInputLayoutElement
    {
        public string text { get; }

        public PlainTextElement(string text) : base("plain_text", null)
        {
            this.text = text;
        }

        public static implicit operator PlainTextElement(string text) =>
            new PlainTextElement(text);

        public static implicit operator string(PlainTextElement element) =>
            element.text;
    }

    /// <summary>
    /// Display Markdown
    /// </summary>
    public class MarkdownElement : Element,
                                   IContextLayoutElement,
                                   ISectionLayoutElement,
                                   IActionsLayoutElement,
                                   IInputLayoutElement
    {
        public string text { get; }

        public MarkdownElement(string text) : base("mrkdwn", null)
        {
            this.text = text;
        }

        public static implicit operator MarkdownElement(string text) =>
            new MarkdownElement(text);

        public static implicit operator string(MarkdownElement element) =>
            element.text;

    }

    /// <summary>
    ///     Display Image on surface
    /// </summary>
    /// <remarks>
    ///     Available on
    ///         Modal, Messages, Home tabs
    ///     Works with
    ///         Section, Action
    /// </remarks>
    public class ImageElement : Element,
                                ISectionLayoutElement,
                                IContextLayoutElement,
                                IActionsLayoutElement,
                                IInputLayoutElement
    {
        public string image_url { get; set; }
        public string alt_text  { get; set; }

        public ImageElement() : base("image", "") { }
        public ImageElement( string actionId, string imageUrl, string altText ) : base( "image", actionId )
        {
            image_url     = imageUrl;
            alt_text = altText;
        }
    }
    
    /// <summary>
    ///     Button Element
    /// </summary>
    /// <remarks>
    ///     Available on
    ///         Modal, Messages, Home tabs
    ///     Works with
    ///         Section, Action
    /// </remarks>
    public class ButtonElement : Element, 
                                 ISectionLayoutElement, 
                                 IActionsLayoutElement
    {
        public PlainText    text    { get; set; }
        public string?      url     { get; set; }
        public string?      value   { get; set; }
        public ButtonStyle? style   { get; set; }
        public Confirm?     confirm { get; set; }

        private event Action<SlackSurface,ButtonElement,BlockActions> ClickEvent;

        public Action<SlackSurface, ButtonElement, BlockActions> Clicked
        {
            set => ClickEvent += value;
        }

        public ButtonElement() : base("button", "" ) { }
        public ButtonElement( string actionId, PlainText text ) : base( "button", actionId )
        {
            this.text = text;
        }

        public void RaiseClicked(SlackSurface surface, BlockActions actions)
        {
            ClickEvent?.Invoke( surface, this, actions );
        }
    }

    public delegate void TextUpdatedHandler(SlackSurface surface, TextInputElement inputElement, TextInputAction action);

    /// <summary>
    ///     Text input Element
    /// </summary>
    /// <remarks>
    ///     Available on
    ///         Modal only
    ///     Works with
    ///         Section, Action, Input
    /// </remarks>
    public class TextInputElement : Element,
                                    IInputLayoutElement
    {
        public PlainText?      placeholder            { get; set; }
        public string?         initial_value          { get; set; }
        public string?         value                  { get; set; }
        public bool?           multiline              { get; set; }
        public int?            min_length             { get; set; }
        public int?            max_length             { get; set; }
        public DispatchAction? dispatch_action_config { get; set; }

        private event TextUpdatedHandler TextUpdatedEvent;

        public TextUpdatedHandler TextUpdated
        {
            set => TextUpdatedEvent += value;
        }


        public TextInputElement() : base("plain_text_input", "") { }
        public TextInputElement( string actionId ) : base( "plain_text_input", actionId )
        {

        }

        public bool ShouldSerializevalue() =>
            false;
        public virtual void RaiseTextUpdatedEvent(SlackSurface surface, TextInputAction action)
        {
            TextUpdatedEvent?.Invoke( surface, this, action);
        }
    }

    /// <summary>
    ///     Multiple Checkbox Element
    /// </summary>
    /// <remarks>
    ///     Available on
    ///         Modal, Messages, Home tabs
    ///     Works with
    ///         Section, Action, Input
    /// </remarks>
    public class CheckboxesElement : Element, 
                                     ISectionLayoutElement, 
                                     IActionsLayoutElement, 
                                     IInputLayoutElement
    {
        public List< Option >  options          { get; set; } = new();
        public List< Option >? initial_options  { get; set; }
        public List< Option >? selected_options { get; set; }
        public Confirm? confirm { get;                 set; }

        public CheckboxesElement() : base("checkboxes", "" ) { }
        public CheckboxesElement(  string actionId, IEnumerable<Option>? options = null ) : base( "checkboxes", actionId )
        {
            if (options != null)
                this.options.AddRange( options );
        }

        public bool ShouldSerializeselected_options() =>
            false;

        public CheckboxesElement Add( Option option )
        {
            options.Add( option );
            return this;
        }
    }

    /// <summary>
    ///     RadioButton group Element
    /// </summary>
    /// <remarks>
    ///     Available on
    ///         Modal, Messages, Home tabs
    ///     Works with
    ///         Section, Action, Input
    /// </remarks>
    public class RadioButtonsElement : Element,
                                       ISectionLayoutElement,
                                       IActionsLayoutElement,
                                       IInputLayoutElement
    {
        public List<Option>  options         { get; set; } = new();
        public List<Option>? initial_options { get; set; }
        public Option?       selected_option { get; set; }
        public Confirm?      confirm         { get; set; }

        public RadioButtonsElement() : base("radio_buttons", "") { }
        public RadioButtonsElement( string actionId, IEnumerable<Option>? options = null) : base("radio_buttons", actionId)
        {
            if (options != null)
                this.options.AddRange(options);
        }

        public bool ShouldSerializeselected_option() =>
            false;

        public RadioButtonsElement Add(Option option)
        {
            options.Add(option);
            return this;
        }
    }

    /// <summary>
    ///     Date Picker Element
    /// </summary>
    /// <remarks>
    ///     Available on
    ///         Modal, Messages, Home tabs
    ///     Works with
    ///         Section, Action, Input
    /// </remarks>
    public class DatePickerElement : Element, 
                                     ISectionLayoutElement,
                                     IActionsLayoutElement,
                                     IInputLayoutElement
    {
        private const string     dateFormat = "yyyy-MM-dd";
        
        public PlainText? placeholder   { get; set; }
        /// <summary>YYYY-MM-DD</summary>
        public string?    initial_date  { get; set; }
        public string?    selected_date { get; set; }
        public Confirm?   confirm       { get; set; }

        [JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public DateTime InitDate
        {
            get => GetInitialDate();
            set => SetInitialDate( value );
        }
        public DatePickerElement() : base("datepicker", "") { }
        public DatePickerElement(  string actionId ) : base( "datepicker", actionId ) { }

        public bool ShouldSerializeselected_date() =>
            false;

        private void SetInitialDate( DateTime date ) =>
            initial_date = date.ToString( dateFormat );

        private DateTime GetInitialDate()
        {
            return string.IsNullOrEmpty(initial_date)
                       ? DateTime.MinValue
                       : DateTime.ParseExact( initial_date, dateFormat, CultureInfo.InvariantCulture);
        }
    }

    /// <summary>
    ///     Time Picker Element
    /// </summary>
    /// <remarks>
    ///     In beta at the time of writing
    ///     Available on
    ///         Modal, Messages, Home tabs
    ///     Works with
    ///         Section, Action, Input
    /// </remarks>
    public class TimePickerElement : Element,
                                     ISectionLayoutElement,
                                     IActionsLayoutElement,
                                     IInputLayoutElement
    {
        private const string timeFormat = "HH:mm";

        public PlainText? placeholder  { get; set; }
        /// <summary>HH:mm</summary>
        public string? initial_time { get;  set; }
        public string? selected_time { get; set; }
        public Confirm? confirm      { get; set; }

        [JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public DateTime InitTime
        {
            get => GetInitialTime();
            set => SetInitialTime(value);
        }

        public TimePickerElement() : base("timepicker", "") { }
        public TimePickerElement( string actionId) : base("timepicker", actionId) { }

        public bool ShouldSerializeselected_time() =>
            false;

        private void SetInitialTime(DateTime time) =>
            initial_time = time.ToString(timeFormat);

        private DateTime GetInitialTime()
        {
            return string.IsNullOrEmpty(initial_time)
                       ? DateTime.MinValue
                       : DateTime.ParseExact(initial_time, timeFormat, CultureInfo.InvariantCulture);
        }
    }


    /// <summary>
    ///     Overflow Menu Element
    /// </summary>
    /// <remarks>
    ///     Available on
    ///         Modal, Messages, Home tabs
    ///     Works with
    ///         Section, Action
    /// </remarks>
    public class OverflowMenuElement : Element,
                                       ISectionLayoutElement,
                                       IActionsLayoutElement
    {
        public List<Option> options         { get; set; } = new();
        public Option?      selected_option { get; set; }
        public Confirm?     confirm         { get; set; }

        private event Action<SlackSurface, OverflowMenuElement, OverflowMenuAction> ClickEvent;

        public Action<SlackSurface, OverflowMenuElement, OverflowMenuAction> Clicked
        {
            set => ClickEvent += value;
        }

        public bool ShouldSerializeselected_option() =>
            false;

        public OverflowMenuElement() : base("overflow", "") { }
        public OverflowMenuElement(  string actionId ) : base( "overflow", actionId ) { }

        public OverflowMenuElement Add(Option option)
        {
            options.Add(option);

            return this;
        }

        public void RaiseClicked(SlackSurface surface, OverflowMenuAction action) =>
            ClickEvent?.Invoke(surface, this, action);
    }

    /// <summary>
    ///     Select Element (dropdown)
    /// </summary>
    /// <remarks>
    ///     Available on
    ///         Modal, Messages, Home tabs
    ///     Works with
    ///         Section, Action, Input
    /// </remarks>
    public class SelectElement : Element,
                                 ISectionLayoutElement,
                                 IActionsLayoutElement,
                                 IInputLayoutElement
    {
        public PlainText          placeholder      { get; set; }
        public List<Option>       options          { get; set; } = new();
        public List<OptionGroup>? option_groups    { get; set; }
        public Option?            initial_option   { get; set; }
        public Option?            selected_option { get; set; }
        public Confirm?           confirm          { get; set; }

        private event Action<SlackSurface, SelectElement, SelectAction> ClickEvent;

        public Action<SlackSurface, SelectElement, SelectAction> Clicked
        {
            set => ClickEvent += value;
        }

        public SelectElement() : base("static_select", "") { }
        public SelectElement(  string actionId, PlainText placeholder ) 
            : base( "static_select", actionId )
        {
            this.placeholder = placeholder;
        }

        public bool ShouldSerializeselected_option() =>
            false;

        public SelectElement Add(Option option)
        {
            options.Add(option);

            return this;
        }
        public SelectElement Add(OptionGroup optionGroup)
        {
            option_groups ??= new List< OptionGroup >();
            option_groups.Add(optionGroup);

            return this;
        }

        public void RaiseClicked(SlackSurface surface, SelectAction action) =>
            ClickEvent?.Invoke(surface, this, action);
    }

    /// <summary>
    ///     Select External Element (dropdown)
    /// </summary>
    /// <remarks>
    ///     Available on
    ///         Modal, Messages, Home tabs
    ///     Works with
    ///         Section, Action, Input
    /// </remarks>
    public class SelectExternalElement : Element,
                                         ISectionLayoutElement,
                                         IActionsLayoutElement,
                                         IInputLayoutElement
    {
        public PlainText          placeholder      { get; set; }
        public Option?            initial_option   { get; set; }
        public Option?            selected_option  { get; set; }
        public int?               min_query_length { get; set; }
        public Confirm?           confirm          { get; set; }

        [JsonIgnore]
        public SuggestionsHandler Suggestions { get; set; }

        private event Action<SlackSurface, SelectExternalElement, SelectExternalAction> ClickEvent;

        public Action<SlackSurface, SelectExternalElement, SelectExternalAction> Clicked
        {
            set => ClickEvent += value;
        }
        public void RaiseClicked(SlackSurface surface, SelectExternalAction action) =>
            ClickEvent?.Invoke(surface, this, action);

        public SelectExternalElement() : base("external_select", "") { }
        public SelectExternalElement( string actionId, PlainText placeholder)
            : base("external_select", actionId)
        {
            this.placeholder = placeholder;
        }

        // json serialisation hint
        public bool ShouldSerializeselected_option() =>
            false;

    }

    /// <summary>
    ///     Select User Element (dropdown)
    /// </summary>
    /// <remarks>
    ///     Available on
    ///         Modal, Messages, Home tabs
    ///     Works with
    ///         Section, Action, Input
    /// </remarks>
    public class UsersSelectElement : Element,
                                      ISectionLayoutElement,
                                      IActionsLayoutElement,
                                      IInputLayoutElement
    {
        public PlainText placeholder   { get; set; }
        public string?   initial_user  { get; set; }
        public string?   selected_user { get; set; }
        public Confirm?  confirm       { get; set; }

        private event Action<SlackSurface, UsersSelectElement, UserSelectAction> ClickEvent;
        public Action<SlackSurface, UsersSelectElement, UserSelectAction> Clicked
        {
            set => ClickEvent += value;
        }
        public void RaiseClicked(SlackSurface surface, UserSelectAction action) =>
            ClickEvent?.Invoke(surface, this, action);

        public UsersSelectElement() : base("users_select", "") { }
        public UsersSelectElement( string actionId, PlainText placeholder)
            : base("users_select", actionId)
        {
            this.placeholder = placeholder;
        }

        public bool ShouldSerializeselected_user() =>
            false;

    }

    /// <summary>
    ///     Select Conversation Element (dropdown)
    /// </summary>
    /// <remarks>
    ///     Public and private channels, DMs and MPIMs
    ///     Available on
    ///         Modal, Messages, Home tabs
    ///     Works with
    ///         Section, Action, Input
    /// </remarks>
    public class ConversationSelectElement : Element,
                                             ISectionLayoutElement,
                                             IActionsLayoutElement,
                                             IInputLayoutElement
    {
        public PlainText placeholder                     { get; set; }
        public string?   initial_conversation            { get; set; }
        public string?   selected_conversation            { get; set; }
        public bool?     default_to_current_conversation { get; set; }
        public Confirm?  confirm                         { get; set; }
        public bool?     response_url_enabled            { get; set; }
        public Filter?   filter                          { get; set; }

        private event Action<SlackSurface, ConversationSelectElement, ConversationSelectAction> ClickEvent;
        public Action<SlackSurface, ConversationSelectElement, ConversationSelectAction> Clicked
        {
            set => ClickEvent += value;
        }
        public void RaiseClicked(SlackSurface surface, ConversationSelectAction action) =>
            ClickEvent?.Invoke(surface, this, action);

        public ConversationSelectElement() : base("conversations_select", "") { }
        public ConversationSelectElement( string actionId, PlainText placeholder)
            : base("conversations_select", actionId)
        {
            this.placeholder = placeholder;
        }

        // json serialisation hint
        public bool ShouldSerializeselected_conversation() =>
            false;

    }

    /// <summary>
    ///     Select Channel Element (dropdown)
    /// </summary>
    /// <remarks>
    ///     Only public channels
    ///     Available on
    ///         Modal, Messages, Home tabs
    ///     Works with
    ///         Section, Action, Input
    /// </remarks>
    public class ChannelSelectElement : Element,
                                        ISectionLayoutElement,
                                        IActionsLayoutElement,
                                        IInputLayoutElement
    {
        public PlainText placeholder          { get; set; }
        public string?   initial_channel      { get; set; }
        public string?   selected_channel     { get; set; }
        public Confirm?  confirm              { get; set; }
        public bool?     response_url_enabled { get; set; }


        private event Action<SlackSurface, ChannelSelectElement, ChannelSelectAction> ClickEvent;
        public Action<SlackSurface, ChannelSelectElement, ChannelSelectAction> Clicked
        {
            set => ClickEvent += value;
        }
        public void RaiseClicked(SlackSurface surface, ChannelSelectAction action)
        {
            ClickEvent?.Invoke(surface, this, action);
        }

        public ChannelSelectElement() : base("channels_select", "") { }
        public ChannelSelectElement( string actionId, PlainText placeholder)
            : base("channels_select", actionId)
        {
            this.placeholder = placeholder;
        }

        // json serialisation hint
        public bool ShouldSerializeselected_channel() =>
            false;
    }






    /// <summary>
    ///     Multi-Select Element (dropdown)
    /// </summary>
    /// <remarks>
    ///     Available on
    ///         Modal, Messages, Home tabs
    ///     Works with
    ///         Section, Input
    /// </remarks>
    public class MultiSelectElement : Element,
                                      ISectionLayoutElement,
                                      IInputLayoutElement
    {
        public PlainText          placeholder        { get; set; }
        public List<Option>       options            { get; set; } = new();
        public List<OptionGroup>? option_groups      { get; set; }
        public List<Option>?      initial_options    { get; set; }
        public List<Option>?      selected_options   { get; set; }
        public Confirm?           confirm            { get; set; }
        public int?               max_selected_items { get; set; }

        private event Action<SlackSurface, MultiSelectElement, MultiSelectAction> ClickEvent;
        public Action<SlackSurface, MultiSelectElement, MultiSelectAction> Clicked
        {
            set => ClickEvent += value;
        }
        public void RaiseClicked(SlackSurface surface, MultiSelectAction action) =>
            ClickEvent?.Invoke(surface, this, action);

        public MultiSelectElement() : base("multi_static_select", "") { }
        public MultiSelectElement( string actionId, PlainText placeholder)
            : base("multi_static_select", actionId)
        {
            this.placeholder = placeholder;
        }

        // json serialisation hint
        public bool ShouldSerializeselected_options() =>
            false;

        public MultiSelectElement Add(Option option)
        {
            options.Add(option);

            return this;
        }
        public MultiSelectElement Add(OptionGroup optionGroup)
        {
            option_groups ??= new List<OptionGroup>();
            option_groups.Add(optionGroup);

            return this;
        }
        public MultiSelectElement AddInitial(Option option)
        {
            initial_options ??= new List< Option >();
            initial_options.Add(option);

            return this;
        }
    }

    /// <summary>
    ///     Multi-Select External Element (dropdown)
    /// </summary>
    /// <remarks>
    ///     Available on
    ///         Modal, Messages, Home tabs
    ///     Works with
    ///         Section, Input
    /// </remarks>
    public class MultiSelectExternalElement : Element,
                                              ISectionLayoutElement,
                                              IInputLayoutElement
    {
        public PlainText          placeholder        { get; set; }
        public List<Option>?      options            { get; set; } = new();
        public List<OptionGroup>? option_groups      { get; set; }
        public List<Option>?      initial_options    { get; set; }
        public List<Option>?      selected_options   { get; set; }
        public int?               min_query_length   { get; set; }
        public Confirm?           confirm            { get; set; }
        public int?               max_selected_items { get; set; }

        [JsonIgnore]
        public SuggestionsHandler Suggestions { get; set; }

        private event Action<SlackSurface, MultiSelectExternalElement, MultiSelectExternalAction> ClickEvent;
        public Action<SlackSurface, MultiSelectExternalElement, MultiSelectExternalAction> Clicked
        {
            set => ClickEvent += value;
        }
        public void RaiseClicked(SlackSurface surface, MultiSelectExternalAction action)
        {
            ClickEvent?.Invoke(surface, this, action);
        }

        public MultiSelectExternalElement() : base("multi_external_select", "") { }
        public MultiSelectExternalElement( string actionId, PlainText placeholder)
            : base("multi_external_select", actionId)
        {
            this.placeholder = placeholder;
        }

        // json serialisation hints
        public bool ShouldSerializeoptions() =>
            false;
        public bool ShouldSerializeselected_options() =>
            false;

    }

    /// <summary>
    ///     Multi-Select Users Element (dropdown)
    /// </summary>
    /// <remarks>
    ///     Available on
    ///         Modal, Messages, Home tabs
    ///     Works with
    ///         Section, Input
    /// </remarks>
    public class MultiUserSelectElement : Element,
                                          ISectionLayoutElement,
                                          IInputLayoutElement
    {
        public PlainText     placeholder        { get; set; }
        public List<string>? initial_users      { get; set; }
        public List<string>? selected_users     { get; set; }
        public Confirm?      confirm            { get; set; }
        public int?          max_selected_items { get; set; }

        private event Action<SlackSurface, MultiUserSelectElement, MultiUserSelectAction> ClickEvent;
        public Action<SlackSurface, MultiUserSelectElement, MultiUserSelectAction> Clicked
        {
            set => ClickEvent += value;
        }
        public void RaiseClicked(SlackSurface surface, MultiUserSelectAction action) =>
            ClickEvent?.Invoke(surface, this, action);

        public MultiUserSelectElement() : base("multi_users_select", "") { }
        public MultiUserSelectElement( string actionId, PlainText placeholder)
            : base("multi_users_select", actionId)
        {
            this.placeholder = placeholder;
        }

        // json serialisation hints
        public bool ShouldSerializeselected_users() =>
            false;

    }

    /// <summary>
    ///     Multi-Select Conversations Element (dropdown)
    /// </summary>
    /// <remarks>
    ///     Public and private channels, DMs and MPIMs
    ///     Available on
    ///         Modal, Messages, Home tabs
    ///     Works with
    ///         Section, Input
    /// </remarks>
    public class MultiConversationSelectElement : Element,
                                                  ISectionLayoutElement,
                                                  IInputLayoutElement
    {
        public PlainText     placeholder                     { get; set; }
        public List<string>? initial_conversations           { get; set; }
        public List<string>? selected_conversations          { get; set; }
        public bool?         default_to_current_conversation { get; set; }
        public Confirm?      confirm                         { get; set; }
        public bool?         response_url_enabled            { get; set; }
        public Filter?       filter                          { get; set; }
        public int?          max_selected_items              { get; set; }

        private event Action<SlackSurface, MultiConversationSelectElement, MultiConversationSelectAction> ClickEvent;
        public Action<SlackSurface, MultiConversationSelectElement, MultiConversationSelectAction> Clicked
        {
            set => ClickEvent += value;
        }
        public void RaiseClicked(SlackSurface surface, MultiConversationSelectAction action) =>
            ClickEvent?.Invoke(surface, this, action);

        public MultiConversationSelectElement() : base("multi_conversations_select", "") { }
        public MultiConversationSelectElement( string actionId, PlainText placeholder)
            : base("multi_conversations_select", actionId)
        {
            this.placeholder = placeholder;
        }

        // json serialisation hint
        public bool ShouldSerializeselected_conversations() =>
            false;

    }

    /// <summary>
    ///     Multi-Select Channels Element (dropdown)
    /// </summary>
    /// <remarks>
    ///     Available on
    ///         Modal, Messages, Home tabs
    ///     Works with
    ///         Section, Input
    /// </remarks>
    public class MultiChannelSelectElement : Element,
                                             ISectionLayoutElement,
                                             IInputLayoutElement
    {
        public PlainText     placeholder          { get; set; }
        public List<string>? initial_channels     { get; set; }
        public List<string>? selected_channels    { get; set; }
        public Confirm?      confirm              { get; set; }
        public bool?         response_url_enabled { get; set; }
        public int?          max_selected_items   { get; set; }

        private event Action<SlackSurface, MultiChannelSelectElement, MultiChannelSelectAction> ClickEvent;
        public Action<SlackSurface, MultiChannelSelectElement, MultiChannelSelectAction> Clicked
        {
            set => ClickEvent += value;
        }
        public void RaiseClicked(SlackSurface surface, MultiChannelSelectAction action) =>
            ClickEvent?.Invoke(surface, this, action);

        public MultiChannelSelectElement() : base("multi_channels_select", "") { }
        public MultiChannelSelectElement( string actionId, PlainText placeholder)
            : base("multi_channels_select", actionId)
        {
            this.placeholder = placeholder;
        }

        // json serialisation hint
        public bool ShouldSerializeselected_channels() =>
            false;
    }

    public class ElementConverter : JsonConverter<IElement?>
    {
        public override bool CanRead =>
            true;

        public override bool CanWrite =>
            false;

        public override void WriteJson(JsonWriter writer, IElement? value, JsonSerializer serializer)
        {
            throw new Exception($"Unable to write Element");
        }

        public override IElement? ReadJson(
            JsonReader     reader,
            Type           objectType,
            IElement?      existingValue,
            bool           hasExistingValue,
            JsonSerializer serializer)
        {
            var jo = JToken.ReadFrom(reader);

            var type = jo[nameof(Element.type)]?.Value<string>();
            switch (type)
            {
            case "image":
                existingValue = new ImageElement();
                break;
            case "button":
                existingValue = new ButtonElement();
                break;
            case "plain_text_input":
                existingValue = new TextInputElement();
                break;
            case "checkboxes":
                existingValue = new CheckboxesElement();
                break;
            case "radio_buttons":
                existingValue = new RadioButtonsElement();
                break;
            case "datepicker":
                existingValue = new DatePickerElement();
                break;
            case "timepicker":
                existingValue = new TimePickerElement();
                break;
            case "overflow":
                existingValue = new OverflowMenuElement();
                break;
            case "static_select":
                existingValue = new SelectElement();
                break;
            case "external_select":
                existingValue = new SelectExternalElement();
                break;
            case "users_select":
                existingValue = new UsersSelectElement();
                break;
            case "conversations_select":
                existingValue = new ConversationSelectElement();
                break;
            case "channels_select":
                existingValue = new ChannelSelectElement();
                break;
            case "multi_static_select":
                existingValue = new MultiSelectElement();
                break;
            case "multi_external_select":
                existingValue = new MultiSelectExternalElement();
                break;
            case "multi_users_select":
                existingValue = new MultiUserSelectElement();
                break;
            case "multi_conversations_select":
                existingValue = new MultiConversationSelectElement();
                break;
            case "multi_channels_select":
                existingValue = new MultiChannelSelectElement();
                break;

            default:
                return default;
            }

            serializer.Populate(jo.CreateReader(), existingValue);
            return existingValue;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Globalization;

using JetBrains.Annotations;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// ReSharper disable InconsistentNaming

#pragma warning disable 8618

namespace SlackForDotNet.Surface
{
    public delegate Options SuggestionsHandler(string value);

    
    [JsonConverter( typeof(ElementConverter))]
    public abstract class Element
    {
        public string type      { get; }
        public string action_id { get; set; }

        protected Element( [ NotNull ] string type, string id )
        {
            this.type = type;
            action_id = id;
        }

    }

    /// <summary>
    /// Display Image on surface
    /// </summary>
    public class ImageElement : Element
    {
        public string image_url { get; set; }
        public string alt_text  { get; set; }

        public ImageElement() : base("image", "") { }
        public ImageElement( string id, string imageUrl, string altText ) : base( "image", id )
        {
            image_url     = imageUrl;
            alt_text = altText;
        }
    }

    public delegate void ClickHandler( SlackSurface surface, ButtonElement button, BlockActions actions );

    /// <summary>
    ///     Button Element
    /// </summary>
    /// <remarks>
    ///     Available on
    ///         Modal, Messages, Home tabs
    ///     Works with
    ///         Section, Action
    /// </remarks>
    public class ButtonElement : Element
    {
        public PlainText    text    { get; set; }
        public string?      url     { get; set; }
        public string?      value   { get; set; }
        public ButtonStyle? style   { get; set; }
        public Confirm?     confirm { get; set; }

        private event ClickHandler ClickEvent;

        public ClickHandler Clicked
        {
            set => ClickEvent += value;
        }

        public ButtonElement() : base("button", "" ) { }
        public ButtonElement( string id, PlainText text ) : base( "button", id )
        {
            this.text = text;
        }

        public void RaiseClicked(SlackSurface surface, BlockActions actions)
        {
            ClickEvent?.Invoke( surface, this, actions );
        }
    }

    /// <summary>
    ///     Text input Element
    /// </summary>
    /// <remarks>
    ///     Available on
    ///         Modal only
    ///     Works with
    ///         Section, Action, Input
    /// </remarks>
    public class TextInputElement : Element
    {
        public PlainText?      placeholder            { get; set; }
        public string?         initial_value          { get; set; }
        public string?         value                  { get; set; }
        public bool?           multiline              { get; set; }
        public int?            min_length             { get; set; }
        public int?            max_length             { get; set; }
        public DispatchAction? dispatch_action_config { get; set; }

        private event EventHandler<string> TextUpdatedEvent;

        public EventHandler<string> TextUpdated
        {
            set => TextUpdatedEvent += value;
        }


        public TextInputElement() : base("plain_text_input", "") { }
        public TextInputElement( string id ) : base( "plain_text_input", id )
        {

        }

        public bool ShouldSerializevalue() =>
            false;
        public virtual void RaiseTextUpdatedEvent(string str)
        {
            TextUpdatedEvent?.Invoke( this, str );
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
    public class CheckboxesElement : Element
    {
        public List< Option >  options          { get; set; } = new();
        public List< Option >? initial_options  { get; set; }
        public List< Option >? selected_options { get; set; }
        public Confirm? confirm { get;                 set; }

        public CheckboxesElement() : base("checkboxes", "" ) { }
        public CheckboxesElement( [ NotNull ] string id, IEnumerable<Option>? options = null ) : base( "checkboxes", id )
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
    public class RadioButtonsElement : Element
    {
        public List<Option>  options         { get; set; } = new();
        public List<Option>? initial_options { get; set; }
        public Option?       selected_option { get; set; }
        public Confirm?      confirm         { get; set; }

        public RadioButtonsElement() : base("radio_buttons", "") { }
        public RadioButtonsElement([NotNull] string id, IEnumerable<Option>? options = null) : base("radio_buttons", id)
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
    public class DatePickerElement : Element
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
        public DatePickerElement( [ NotNull ] string id ) : base( "datepicker", id ) { }

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
    public class TimePickerElement : Element
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
        public TimePickerElement([NotNull] string id) : base("timepicker", id) { }

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
    public class OverflowMenuElement : Element
    {
        public List<Option> options         { get; set; } = new();
        public Option?      selected_option { get; set; }
        public Confirm?     confirm         { get; set; }

        private event EventHandler<string> ClickEvent;

        public EventHandler<string> Clicked
        {
            set => ClickEvent += value;
        }

        public bool ShouldSerializeselected_option() =>
            false;

        public OverflowMenuElement() : base("overflow", "") { }
        public OverflowMenuElement( [ NotNull ] string id ) : base( "overflow", id ) { }

        public OverflowMenuElement Add(Option option)
        {
            options.Add(option);

            return this;
        }

        public void RaiseClicked(string id)
        {
            ClickEvent?.Invoke(this, id);
        }
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
    public class SelectElement : Element
    {
        public PlainText          placeholder      { get; set; }
        public List<Option>       options          { get; set; } = new();
        public List<OptionGroup>? option_groups    { get; set; }
        public Option?            initial_option   { get; set; }
        public Option?            selected_option { get; set; }
        public Confirm?           confirm          { get; set; }

        private event EventHandler<string> ClickEvent;

        public EventHandler<string> Clicked
        {
            set => ClickEvent += value;
        }

        public SelectElement() : base("static_select", "") { }
        public SelectElement( [ NotNull ] string id, PlainText placeholder ) 
            : base( "static_select", id )
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

        public void RaiseClicked(string id)
        {
            ClickEvent?.Invoke(this, id);
        }

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
    public class SelectExternalElement : Element
    {
        public PlainText          placeholder      { get; set; }
        public Option?            initial_option   { get; set; }
        public Option?            selected_option  { get; set; }
        public int?               min_query_length { get; set; }
        public Confirm?           confirm          { get; set; }

        [JsonIgnore]
        public SuggestionsHandler Suggestions { get; set; }

        private event EventHandler<string> ClickEvent;
        public EventHandler<string> Clicked
        {
            set => ClickEvent += value;
        }
        public void RaiseClicked(string id)
        {
            ClickEvent?.Invoke(this, id);
        }


        public SelectExternalElement() : base("external_select", "") { }
        public SelectExternalElement([NotNull] string id, PlainText placeholder)
            : base("external_select", id)
        {
            this.placeholder = placeholder;
        }

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
    public class UsersSelectElement : Element
    {
        public PlainText placeholder   { get; set; }
        public string?   initial_user  { get; set; }
        public string?   selected_user { get; set; }
        public Confirm?  confirm       { get; set; }

        private event EventHandler<string> ClickEvent;
        public EventHandler<string> Clicked
        {
            set => ClickEvent += value;
        }
        public void RaiseClicked(string id)
        {
            ClickEvent?.Invoke(this, id);
        }

        public UsersSelectElement() : base("users_select", "") { }
        public UsersSelectElement([NotNull] string id, PlainText placeholder)
            : base("users_select", id)
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
    public class ConversationSelectElement : Element
    {
        public PlainText placeholder                     { get; set; }
        public string?   initial_conversation            { get; set; }
        public string?   selected_conversation            { get; set; }
        public bool?     default_to_current_conversation { get; set; }
        public Confirm?  confirm                         { get; set; }
        public bool?     response_url_enabled            { get; set; }
        public Filter?   filter                          { get; set; }

        private event EventHandler<string> ClickEvent;
        public EventHandler<string> Clicked
        {
            set => ClickEvent += value;
        }
        public void RaiseClicked(string id)
        {
            ClickEvent?.Invoke(this, id);
        }

        public ConversationSelectElement() : base("conversations_select", "") { }
        public ConversationSelectElement([NotNull] string id, PlainText placeholder)
            : base("conversations_select", id)
        {
            this.placeholder = placeholder;
        }

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
    public class ChannelSelectElement : Element
    {
        public PlainText placeholder          { get; set; }
        public string?   initial_channel      { get; set; }
        public string?   selected_channel     { get; set; }
        public Confirm?  confirm              { get; set; }
        public bool?     response_url_enabled { get; set; }


        private event EventHandler<string> ClickEvent;
        public EventHandler<string> Clicked
        {
            set => ClickEvent += value;
        }
        public void RaiseClicked(string id)
        {
            ClickEvent?.Invoke(this, id);
        }

        public ChannelSelectElement() : base("channels_select", "") { }
        public ChannelSelectElement([NotNull] string id, PlainText placeholder)
            : base("channels_select", id)
        {
            this.placeholder = placeholder;
        }

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
    ///         Section, Action, Input
    /// </remarks>
    public class MultiSelectElement : Element
    {
        public PlainText          placeholder        { get; set; }
        public List<Option>       options            { get; set; } = new();
        public List<OptionGroup>? option_groups      { get; set; }
        public List<Option>?      initial_options    { get; set; }
        public List<Option>?      selected_options   { get; set; }
        public Confirm?           confirm            { get; set; }
        public int?               max_selected_items { get; set; }

        private event EventHandler<List<Option>> ClickEvent;
        public EventHandler<List<Option>> Clicked
        {
            set => ClickEvent += value;
        }
        public void RaiseClicked(List<Option> options)
        {
            ClickEvent?.Invoke(this, options);
        }

        public MultiSelectElement() : base("multi_static_select", "") { }
        public MultiSelectElement([NotNull] string id, PlainText placeholder)
            : base("multi_static_select", id)
        {
            this.placeholder = placeholder;
        }

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
    ///         Section, Action, Input
    /// </remarks>
    public class MultiSelectExternalElement : Element
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

        private event EventHandler<List<Option>> ClickEvent;
        public EventHandler<List<Option>> Clicked
        {
            set => ClickEvent += value;
        }
        public void RaiseClicked(List<Option> options)
        {
            ClickEvent?.Invoke(this, options);
        }

        public MultiSelectExternalElement() : base("multi_external_select", "") { }
        public MultiSelectExternalElement([NotNull] string id, PlainText placeholder)
            : base("multi_external_select", id)
        {
            this.placeholder = placeholder;
        }

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
    ///         Section, Action, Input
    /// </remarks>
    public class MultiUserSelectElement : Element
    {
        public PlainText     placeholder        { get; set; }
        public List<string>? initial_users      { get; set; }
        public List<string>? selected_users     { get; set; }
        public Confirm?      confirm            { get; set; }
        public int?          max_selected_items { get; set; }

        private event EventHandler<List<string>> ClickEvent;
        public EventHandler<List<string>> Clicked
        {
            set => ClickEvent += value;
        }
        public void RaiseClicked(List<string> ids)
        {
            ClickEvent?.Invoke(this, ids);
        }

        public MultiUserSelectElement() : base("multi_users_select", "") { }
        public MultiUserSelectElement([NotNull] string id, PlainText placeholder)
            : base("multi_users_select", id)
        {
            this.placeholder = placeholder;
        }

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
    ///         Section, Action, Input
    /// </remarks>
    public class MultiConversationSelectElement : Element
    {
        public PlainText     placeholder                     { get; set; }
        public List<string>? initial_conversations           { get; set; }
        public List<string>? selected_conversations          { get; set; }
        public bool?         default_to_current_conversation { get; set; }
        public Confirm?      confirm                         { get; set; }
        public bool?         response_url_enabled            { get; set; }
        public Filter?       filter                          { get; set; }
        public int?          max_selected_items              { get; set; }

        private event EventHandler<List<string>> ClickEvent;
        public EventHandler<List<string>> Clicked
        {
            set => ClickEvent += value;
        }
        public void RaiseClicked(List<string> ids)
        {
            ClickEvent?.Invoke(this, ids);
        }

        public MultiConversationSelectElement() : base("multi_conversations_select", "") { }
        public MultiConversationSelectElement([NotNull] string id, PlainText placeholder)
            : base("multi_conversations_select", id)
        {
            this.placeholder = placeholder;
        }

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
    ///         Section, Action, Input
    /// </remarks>
    public class MultiChannelSelectElement : Element
    {
        public PlainText     placeholder          { get; set; }
        public List<string>? initial_channels     { get; set; }
        public List<string>? selected_channels    { get; set; }
        public Confirm?      confirm              { get; set; }
        public bool?         response_url_enabled { get; set; }
        public int?          max_selected_items   { get; set; }

        private event EventHandler<List<string>> ClickEvent;
        public EventHandler<List<string>> Clicked
        {
            set => ClickEvent += value;
        }
        public void RaiseClicked(List<string> ids)
        {
            ClickEvent?.Invoke(this, ids);
        }

        public MultiChannelSelectElement() : base("multi_channels_select", "") { }
        public MultiChannelSelectElement([NotNull] string id, PlainText placeholder)
            : base("multi_channels_select", id)
        {
            this.placeholder = placeholder;
        }

        public bool ShouldSerializeselected_channels() =>
            false;
    }

    public class ElementConverter : JsonConverter<Element?>
    {
        public override bool CanRead =>
            true;

        public override bool CanWrite =>
            false;

        public override void WriteJson(JsonWriter writer, Element? value, JsonSerializer serializer)
        {
            throw new Exception($"Unable to write Element");
        }

        public override Element? ReadJson(
            JsonReader     reader,
            Type           objectType,
            Element?       existingValue,
            bool           hasExistingValue,
            JsonSerializer serializer)
        {
            var jo = JObject.ReadFrom(reader);

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

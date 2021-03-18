using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#pragma warning disable 8618

// ReSharper disable InconsistentNaming

namespace SlackForDotNet.Surface
{
    public enum ButtonStyle
    {
        @default,
        primary,
        danger
    }

    [JsonConverter( typeof(SurfaceTextConverter))]
    public abstract class Text
    {
        public string type     { get; }
        public string text     { get; set; }
        public bool?  emoji    { get; set; }
        public bool?  verbatim { get; set; }

        protected Text( [ NotNull ] string type, string text )
        {
            this.type = type;
            this.text = text;
        }

        public static implicit operator Text(string text) =>
            new PlainText(text);

        public static implicit operator string(Text text) =>
            text.text;

    }

    public class PlainText : Text
    {
        public PlainText() : base("plain_text", "") { }
        public PlainText(string text) : base("plain_text", text) { }
        
        public static implicit operator PlainText(string text) =>
            new (text);

        public override string ToString() =>
            text;
    }
    public class Markdown : Text
    {
        public Markdown() : base("mrkdwn", "") { }
        public Markdown(string text) : base("mrkdwn", text) { }
    }

    public class Confirm
    {
        public PlainText    title   { get; set; }
        public Text         text    { get; set; }
        public PlainText    confirm { get; set; }
        public PlainText    deny    { get; set; }
        
        [JsonIgnore]
        public ButtonStyle? buttonStyle   { get; set; }

        public string? style
        {
            get
            {
                switch (buttonStyle)
                {
                    case ButtonStyle.primary:
                    case ButtonStyle.danger:
                        return buttonStyle.ToString();
                }

                return default;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    buttonStyle = null;
                else
                {
                    buttonStyle = Enum.TryParse( value, out ButtonStyle bs )
                                      ? bs
                                      : null;
                }
            }
        }

        public Confirm() { }
        public Confirm( PlainText title, Text text, PlainText confirm, PlainText deny )
        {
            this.title   = title;
            this.text    = text;
            this.confirm = confirm;
            this.deny    = deny;
        }
    }

    public class Option
    {
        public Text       text        { get; set; }
        public string     value       { get; set; }
        public PlainText? description { get; set; }
        public string?    url         { get; set; }

        public Option() { }
        public Option( Text text, string value )
        {
            this.text  = text;
            this.value = value;
        }

        public override string ToString() =>
            $"{text.text} ({value})";
    }

    public class Options
    {
        public List<Option> options { get; set; } = new();
    }

    public class OptionGroup
    {
        public PlainText      label   { get; set; }
        public List< Option > options { get; set; } = new();

        public OptionGroup() { }
        public OptionGroup( PlainText label ) { this.label = label; }
        public OptionGroup Add( Option option )
        {
            options.Add( option );
            return this;
        }
    }

    public class DispatchAction
    {
        private const string OnEnterPressed     = "on_enter_pressed";
        private const string OnCharacterEntered = "on_character_entered";

        public static readonly DispatchAction EnterPressed = new (OnEnterPressed);
        public static readonly DispatchAction CharacterEntered = new (OnCharacterEntered);

        public List< string >? trigger_actions_on { get; set; } = new();

        public DispatchAction() { }
        public DispatchAction(params string[] values)
        {
            if (values.Length == 0)
                return;

            foreach (var value in values)
            {
                if (trigger_actions_on?.Contains( value ) == false)
                    trigger_actions_on.Add( value );
            }
        }
    }

    public class Filter
    {
        public List<string>? include                          { get; set; }
        public bool?         exclude_external_shared_channels { get; set; }
        public bool?         exclude_bot_users                { get; set; }

        public static implicit operator Filter(string[] values) =>
            new  Filter { include = values.ToList() };
    }






    public class SurfaceTextConverter : JsonConverter<Text?>
    {
        public override bool CanRead =>
            true;

        public override bool CanWrite =>
            false;

        public override void WriteJson(JsonWriter writer, Text? value, JsonSerializer serializer) =>
            throw new Exception($"Unable to write Text");

        public override Text? ReadJson(
            JsonReader     reader,
            Type           objectType,
            Text?          existingValue,
            bool           hasExistingValue,
            JsonSerializer serializer)
        {
            var jo = JObject.ReadFrom(reader);

            var  type = jo[nameof(Text.type)]?.Value<string>();
            switch (type)
            {
                case "plain_text":
                    existingValue = new PlainText();
                    break;
                case "mrkdwn":
                    existingValue = new Markdown();
                    break;
                default:
                    return default;
            }

            serializer.Populate(jo.CreateReader(), existingValue);
            return existingValue;
        }
    }
}

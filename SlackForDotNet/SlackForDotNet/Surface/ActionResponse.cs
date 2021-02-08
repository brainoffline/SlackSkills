using System;
using System.Collections.Generic;
using System.Globalization;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
// ReSharper disable InconsistentNaming

namespace SlackForDotNet.Surface
{
    [JsonConverter( typeof(ActionResponseConverter))]
    public abstract class ActionResponse
    {
        #pragma warning disable 8618
        public string type      { get; set; }
        public string block_id  { get; set; }
        public string action_id { get; set; }
        public string action_ts { get; set; }
        #pragma warning restore 8618
    }
    public class ButtonAction : ActionResponse
    {
        public Text?   text  { get; set; }
        public string? value { get; set; }
    }

    public class DatePickerAction : ActionResponse
    {
        private const string  dateFormat = "yyyy-MM-dd";
        
        public        string? selected_date { get; set; }
        public        string? initial_date  { get; set; }

        public DateTime SelectedDate =>
            ConvertDate(selected_date);
        public DateTime InitialDate =>
            ConvertDate(initial_date);

        private DateTime ConvertDate(string? str)
        {
            return string.IsNullOrEmpty(str)
                       ? DateTime.MinValue
                       : DateTime.ParseExact(str, dateFormat, CultureInfo.InvariantCulture);
        }
    }

    public class TimePickerAction : ActionResponse
    {
        private const string timeFormat = "HH:mm";

        public string? selected_time { get; set; }
        public string? initial_time  { get; set; }

        public DateTime SelectedTime =>
            ConvertTime(selected_time);
        public DateTime InitialTime =>
            ConvertTime(initial_time);

        private DateTime ConvertTime(string? str)
        {
            return string.IsNullOrEmpty(str)
                       ? DateTime.MinValue
                       : DateTime.ParseExact(str, timeFormat, CultureInfo.InvariantCulture);
        }
    }

    public class TextInputAction : ActionResponse
    {
        public string? value { get; set; }
    }
    public class RadioButtonsAction : ActionResponse
    {
        public Option? selected_option { get; set; }
    }
    public class CheckboxesAction : ActionResponse
    {
        public List<Option>? selected_options { get; set; }
    }

    public class OverflowMenuAction : ActionResponse
    {
        public Option?   selected_option { get; set; }
    }
    public class SelectAction : ActionResponse
    {
        public PlainText? placeholder     { get; set; }
        public Option?    selected_option { get; set; }
    }
    public class SelectExternalAction : ActionResponse
    {
        public PlainText? placeholder     { get; set; }
        public Option?    selected_option { get; set; }
    }
    public class UserSelectAction : ActionResponse
    {
        public string? selected_user { get; set; }
    }
    public class ConversationSelectAction : ActionResponse
    {
        public string? selected_conversation { get; set; }
    }
    public class ChannelSelectAction : ActionResponse
    {
        public string? selected_channel { get; set; }
    }
    public class MultiSelectAction : ActionResponse
    {
        public PlainText?    placeholder      { get; set; }
        public List<Option>? selected_options { get; set; }
    }
    public class MultiSelectExternalAction : ActionResponse
    {
        public PlainText?    placeholder      { get; set; }
        public List<Option>? selected_options { get; set; }
    }
    public class MultiUserSelectAction : ActionResponse
    {
        public List<string>? selected_users { get; set; }
    }
    public class MultiConversationSelectAction : ActionResponse
    {
        public List<string>? selected_conversations { get; set; }
    }
    public class MultiChannelSelectAction : ActionResponse
    {
        public List<string>? selected_channels { get; set; }
    }









    public class ActionResponseConverter : JsonConverter<ActionResponse?>
    {
        public override bool CanRead =>
            true;

        public override bool CanWrite =>
            false;

        public override void WriteJson(JsonWriter writer, ActionResponse? value, JsonSerializer serializer)
        {
            throw new Exception($"Unable to write Element");
        }

        public override ActionResponse? ReadJson(
            JsonReader      reader,
            Type            objectType,
            ActionResponse? existingValue,
            bool            hasExistingValue,
            JsonSerializer  serializer)
        {
            var jo = JObject.ReadFrom(reader);

            var type = jo[nameof(Element.type)]?.Value<string>();
            switch (type)
            {
                case "image":
                    return default;
                case "button":
                    existingValue = new ButtonAction();
                    break;
                case "plain_text_input":
                    existingValue = new TextInputAction();
                    break;
                case "checkboxes":
                    existingValue = new CheckboxesAction();
                    break;
                case "radio_buttons":
                    existingValue = new RadioButtonsAction();
                    break;
                case "datepicker":
                    existingValue = new DatePickerAction();
                    break;
                case "timepicker":
                    existingValue = new TimePickerAction();
                    break;
                case "overflow":
                    existingValue = new OverflowMenuAction();
                    break;
                case "static_select":
                    existingValue = new SelectAction();
                    break;
                case "external_select":
                    existingValue = new SelectExternalAction();
                    break;
                case "users_select":
                    existingValue = new UserSelectAction();
                    break;
                case "conversations_select":
                    existingValue = new ConversationSelectAction();
                    break;
                case "channels_select":
                    existingValue = new ChannelSelectAction();
                    break;
                case "multi_static_select":
                    existingValue = new MultiSelectAction();
                    break;
                case "multi_external_select":
                    existingValue = new MultiSelectExternalAction();
                    break;
                case "multi_users_select":
                    existingValue = new MultiUserSelectAction();
                    break;
                case "multi_conversations_select":
                    existingValue = new MultiConversationSelectAction();
                    break;
                case "multi_channels_select":
                    existingValue = new MultiChannelSelectAction();
                    break;

                default:
                    return default;
            }

            serializer.Populate(jo.CreateReader(), existingValue);
            return existingValue;
        }

    }
}

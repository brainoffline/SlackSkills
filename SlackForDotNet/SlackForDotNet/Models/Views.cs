using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using SlackForDotNet.Surface;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized

namespace SlackForDotNet
{
    /// <summary>
    ///     Possible values: home, modal
    /// </summary>
    [JsonConverter( typeof(ViewConverter))]
    public class View 
    {
        public string          type                  { get; set; }
        public string?         previous_view_id      { get; set; }
        public List< Layout >? blocks                { get; set; }
        public string?         private_metadata      { get; set; }
        public string?         id                    { get; set; }
        public string?         callback_id           { get; set; }
        public string?         external_id           { get; set; }
        public string?         root_view_id          { get; set; }
        public string?         app_id                { get; set; }
        public string?         bot_id                { get; set; }
        public string?         team_id               { get; set; }
        public string?         app_installed_team_id { get; set; }
        public State?          state                 { get; set; }
        public string?         hash                  { get; set; }

        public View Add( Layout block )
        {
            blocks ??= new List< Layout >();
            blocks.Add( block );

            return this;
        }

        internal View() { } // pseudo abstract outside library

        public View Duplicate( )
        {
            View view = (View)MemberwiseClone();
            view.id           = null;
            view.root_view_id = null;
            view.app_id       = null;
            view.bot_id       = null;
            view.team_id      = null;
            view.hash         = null;

            return view;
        }

        public class State
        {
            public object values { get; set; }
            //public Dictionary<string, Dictionary<string,Surface.Element> > values { get; set; }
        }
    }

    public class Blocks : SlackMessage
    {
        public List<Layout>? blocks { get; set; }

        public Blocks Add(Layout block)
        {
            blocks ??= new List<Layout>();
            blocks.Add(block);

            return this;
        }
    }

    public class HometabView : View
    {
        public HometabView()
        {
            type = "home";
        }
    }

    public class ModalView : View
    {
        public PlainText  title           { get; set; }
        public PlainText? close           { get; set; }
        public PlainText? submit          { get; set; }
        public bool?      clear_on_close  { get; set; }
        public bool?      notify_on_close { get; set; }
        public bool?      submit_disabled { get; set; } // Primary for configuration modals

        public ModalView()
        {
            type = "modal";
        }
    }

    public class ViewResponseAction
    {
        public string                    response_action { get; set; } // update, push, errors
        public View                      view            { get; set; }
        public Dictionary<string,string> errors          { get; set; }  // block-id, message
    }


    public class ViewConverter : JsonConverter<View?>
    {
        public override bool CanRead =>
            true;

        public override bool CanWrite =>
            false;

        public override void WriteJson(JsonWriter writer, View? value, JsonSerializer serializer) =>
            throw new Exception($"Unable to write View");

        public override View? ReadJson(
            JsonReader     reader,
            Type           objectType,
            View?          existingValue,
            bool           hasExistingValue,
            JsonSerializer serializer)
        {
            var jo = JObject.ReadFrom(reader);

            var type = jo[nameof(Text.type)]?.Value<string>();
            switch (type)
            {
            case "home":
                existingValue = new HometabView();
                break;
            case "modal":
                existingValue = new ModalView();
                break;
            default:
                return default;
            }

            serializer.Populate(jo.CreateReader(), existingValue);
            return existingValue;
        }

    }
}

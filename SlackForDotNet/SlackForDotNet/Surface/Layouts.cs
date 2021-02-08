using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
#pragma warning disable 8618

// ReSharper disable InconsistentNaming

namespace SlackForDotNet.Surface
{
    [JsonConverter( typeof(LayoutConverter))]
    public abstract class Layout
    {
        public string  type     { get; }
        public string? block_id { get; set; }

        protected Layout( string type ) { this.type = type; }
    }

    /// <summary>
    ///     Header Layout
    /// </summary>
    /// <remarks>
    ///     Available in
    ///         Modal, Messages, Home tabs
    /// </remarks>
    public class HeaderLayout : Layout
    {
        public PlainText text { get; set; }

        public HeaderLayout() : base("header") { }
        public HeaderLayout( [ NotNull ] PlainText text ) : base( "header" )
        {
            this.text = text;
        }
    }

    /// <summary>
    ///     Context Layout
    /// </summary>
    /// <remarks>
    ///     Available in
    ///         Modal, Messages, Home tabs
    /// </remarks>
    public class ContextLayout : Layout
    {
        public List< Element > elements { get; set; } = new ();
        
        public ContextLayout() : base( "context" ) { }

        public ContextLayout Add( Element element )
        {
            elements.Add( element );

            return this;
        }
    }

    /// <summary>
    ///     Image Layout
    /// </summary>
    /// <remarks>
    ///     Available in
    ///         Modal, Messages, Home tabs
    /// </remarks>
    public class ImageLayout : Layout
    {
        public string     image_url { get; set; }
        public string     alt_text  { get; set; }
        public PlainText? title     { get; set; }

        public ImageLayout() : base("image") { }
        public ImageLayout( string imageUrl, string altText ) : base( "image" )
        {
            image_url     = imageUrl;
            alt_text = altText;
        }
    }

    /// <summary>
    ///     Divider
    /// </summary>
    /// <remarks>
    ///     Available in
    ///         Modal, Messages, Home tabs
    /// </remarks>
    public class DividerLayout : Layout
    {
        public DividerLayout( ) : base( "divider" ) { }
    }

    /// <summary>
    ///     Section Layout
    /// </summary>
    /// <remarks>
    ///     Available in
    ///         Modal, Messages, Home tabs
    /// </remarks>
    public class SectionLayout : Layout
    {
        public Text?       text      { get; set; }
        public List<Text>? fields    { get; set; }
        public Element?    accessory { get; set; }
        
        public SectionLayout( ) : base( "section" ) { }

        public SectionLayout Add( Text field )
        {
            fields ??= new List< Text >();
            fields.Add( field );

            return this;
        }
    }

    /// <summary>
    ///     Input Layout.  Single element
    /// </summary>
    /// <remarks>
    ///     Available in
    ///         Modal, Home tabs
    /// </remarks>
    public class InputLayout : Layout
    {
        public PlainText  label           { get; set; }
        public Element    element         { get; set; }
        public bool?      dispatch_action { get; set; }
        public PlainText? hint            { get; set; }
        public bool?      optional        { get; set; }

        public InputLayout() : base("input") { }
        public InputLayout( PlainText label, Element element ) : base( "input" )
        {
            this.label   = label;
            this.element = element;
        }
    }

    /// <summary>
    ///     Action Layout. Multiple elements
    /// </summary>
    /// <remarks>
    ///     Available in
    ///         Modal, Messages, Home tabs
    /// </remarks>
    public class ActionLayout : Layout
    {
        public string          action_id { get; set; }
        public PlainText       label     { get; set; }
        public List< Element > elements  { get; set; } = new();

        public ActionLayout() : base("action") { }
        public ActionLayout(PlainText label) : base("action")
        {
            this.label   = label;
        }

        public ActionLayout Add( Element element )
        {
            elements.Add( element );

            return this;
        }
    }

    /// <summary>
    ///     File Layout
    /// </summary>
    /// <remarks>
    ///     Available in
    ///         Messages
    /// </remarks>
    public class FileLayout : Layout
    {
        public string external_id { get; set; }
        public string source      { get; } = "remote";
        
        public FileLayout( string externalId) : base( "file" )
        {
            external_id = externalId;
        }
    }


    public class LayoutConverter : JsonConverter<Layout?>
    {
        public override bool CanRead =>
            true;

        public override bool CanWrite =>
            false;

        public override void WriteJson(JsonWriter writer, Layout? value, JsonSerializer serializer)
        {
            throw new Exception($"Unable to write Layout");
        }

        public override Layout? ReadJson(
            JsonReader reader,
            Type objectType,
            Layout? existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            var jo = JObject.ReadFrom(reader);

            var type = jo[nameof(Layout.type)]?.Value<string>();
            switch (type)
            {
            case "header":
                existingValue = new HeaderLayout();
                break;
            case "context":
                existingValue = new ContextLayout();
                break;
            case "image":
                existingValue = new ImageLayout();
                break;
            case "divider":
                existingValue = new DividerLayout();
                break;
            case "section":
                existingValue = new SectionLayout();
                break;
            case "input":
                existingValue = new InputLayout();
                break;
            case "action":
                existingValue = new ActionLayout();
                break;

                default:
                    return default;
            }

            serializer.Populate(jo.CreateReader(), existingValue);
            return existingValue;
        }

    }
}

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
    public class Attachment
    {
        /// <summary>
        /// Possible values; default
        /// </summary>
        public string attachment_type { get; set; }

        public string  fallback    { get; set; }
        public string  color       { get; set; }
        public string  pretext     { get; set; }
        public string  author_name { get; set; }
        public string  author_link { get; set; }
        public string  author_icon { get; set; }
        public string  title       { get; set; }
        public string  title_link  { get; set; }
        public string  text        { get; set; }
        public Field[] fields      { get; set; }
        public Block[] actions     { get; set; }
        public string  image_url   { get; set; }
        public string  thumb_url   { get; set; }
        public string  footer      { get; set; }
        public string  footer_icon { get; set; }
        public int     ts          { get; set; }

        public class Field
        {
            public string Title { get; set; }
            public string Value { get; set; }
            public bool?  Short { get; set; }
        }
    }
}

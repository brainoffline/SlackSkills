// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

using System;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

namespace SlackSkills
{
    public class Reminder
    {
        public int    id          { get; set; }
        public string creator     { get; set; }
        public string user        { get; set; }
        public string text        { get; set; }
        public bool?  recurring   { get; set; }
        public int?   time        { get; set; }
        public int?   complete_ts { get; set; }
    }
}

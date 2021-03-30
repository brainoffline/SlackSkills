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
    public class Item
    {
        public string      type       { get; set; }
        public string      channel    { get; set; }
        public int?        created    { get; set; }
        public string      created_by { get; set; }
        public MessageBase message    { get; set; }
    }
}

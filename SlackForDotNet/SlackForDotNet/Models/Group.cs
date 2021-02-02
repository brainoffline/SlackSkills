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
    public class Group
    {
        public string id      { get; set; }
        public string name    { get; set; }
        public int?   created { get; set; }
        public string creator { get; set; }

        public bool? is_group    { get; set; }
        public bool? is_archived { get; set; }
        public bool? is_mpim     { get; set; }

        public MessageBase         latest  { get; set; }
        public OwnedStampedMessage topic   { get; set; }
        public OwnedStampedMessage purpose { get; set; }
        public string[]?           members { get; set; }

        public string last_read            { get; set; }
        public int?   unread_count         { get; set; }
        public int?   unread_count_display { get; set; }
    }
}

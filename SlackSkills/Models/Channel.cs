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
    public class Channel
    {
        public string  id              { get; set; }
        public string  name            { get; set; }
        public int?    created         { get; set; }
        public string? creator         { get; set; }
        public string  name_normalized { get; set; }

        public bool? is_archived   { get; set; }
        public bool? is_channel    { get; set; }
        public bool? is_ext_shared { get; set; }
        public bool? is_general    { get; set; }
        public bool? is_group      { get; set; }
        public bool? is_im         { get; set; }
        public bool? is_member     { get; set; }
        public bool? is_mpim       { get; set; }
        public bool? is_open       { get; set; }
        public bool? is_org_shared { get; set; }
        public bool? is_private    { get; set; }
        public bool? is_shared     { get; set; }
        public bool? is_starred    { get; set; }

        public MessageBase         latest  { get; set; }
        public OwnedStampedMessage topic   { get; set; }
        public OwnedStampedMessage purpose { get; set; }

        public string? last_read            { get; set; }
        public int?    unread_count         { get; set; }
        public int?    unread_count_display { get; set; }
        public int?    priority             { get; set; }
        public int?    num_members          { get; set; }
        public string  locale               { get; set; }

        public string[]? previous_names { get; set; }
        public string[]? members        { get; set; }
    }
}

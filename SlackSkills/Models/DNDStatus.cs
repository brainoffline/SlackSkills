// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

using System;

namespace SlackSkills
{
    public class DNDStatus
    {
        public bool? dnd_enabled       { get; set; }
        public int?  next_dnd_start_ts { get; set; }
        public int?  next_dnd_end_ts   { get; set; }
        public bool? snooze_enabled    { get; set; }
        public int?  snooze_endtime    { get; set; }
        public int?  snooze_remaining  { get; set; }
    }
}

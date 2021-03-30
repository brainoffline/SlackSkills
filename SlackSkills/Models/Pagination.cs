// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

using System;

namespace SlackSkills
{
    public class Pagination
    {
        public int first       { get; set; }
        public int last        { get; set; }
        public int page        { get; set; }
        public int page_count  { get; set; }
        public int per_page    { get; set; }
        public int total_count { get; set; }
    }
}

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

using System;

namespace SlackForDotNet
{
    public class Paging
    {
        public int count { get; set; }
        public int total { get; set; }
        public int page  { get; set; }
        public int pages { get; set; }
    }
}

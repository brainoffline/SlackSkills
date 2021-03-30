using System;
using System.Collections.Generic;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

namespace SlackSkills
{
    public class Filter
    {
        
        public List< string >? include { get; set; }

        public bool? exclude_external_shared_channels { get; set; }
        public bool? exclude_bot_users                { get; set; }
    }
}

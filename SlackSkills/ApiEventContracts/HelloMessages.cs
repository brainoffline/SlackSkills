// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

using System;

namespace SlackSkills
{
    [ SlackMessage( "hello" ) ]
    public class HelloResponse : SlackMessage
    {
        public int             num_connections { get; set; }
        public DebugInfo?      debug_info      { get; set; }
        public ConnectionInfo? connection_info { get; set; }
    }

    public class DebugInfo
    {
        public string? host                        { get; set; }
        public int?    build_number                { get; set; }
        public int?    approximate_connection_time { get; set; }
    }
    
    public class ConnectionInfo
    {
        public string? app_id { get; set; }
    }
}

using System;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

namespace SlackSkills
{
    [ SlackMessage( "file_change", apiType: Msg.RTM | Msg.Event, scope: "file:read" ) ]
    public class FileChange : SlackMessage
    {
        public string    file_id { get; set; }
        public SlackFile file    { get; set; }
    }

    [SlackMessage( "file_comment_added", apiType: Msg.RTM | Msg.Event, scope: "file:read")]
    public class FileCommentAdded : SlackMessage
    {
        public string    file_id { get; set; }
        public Comment   comment { get; set; }
        public SlackFile file    { get; set; }
    }

    [SlackMessage( "file_comment_deleted", apiType: Msg.RTM | Msg.Event, scope: "file:read")]
    public class FileCommentDeleted : SlackMessage
    {
        public string    file_id { get; set; }
        public string    comment { get; set; }
        public SlackFile file    { get; set; }
    }


    [SlackMessage( "file_created", apiType: Msg.RTM | Msg.Event, scope: "file:read" ) ]
    public class FileCreated : SlackMessage
    {
        public string    file_id { get; set; }
        public SlackFile file    { get; set; }
    }

    [ SlackMessage( "file_deleted", apiType: Msg.RTM | Msg.Event, scope: "file:read" ) ]
    public class FileDeleted : SlackMessage
    {
        public string file_id  { get; set; }
    }

    [ SlackMessage( "file_public", apiType: Msg.RTM | Msg.Event, scope: "file:read" ) ]
    public class FilePublic : SlackMessage
    {
        public string    file_id { get; set; }
        public SlackFile file    { get; set; }
    }

    [ SlackMessage( "file_shared", apiType: Msg.RTM | Msg.Event, scope: "file:read" ) ]
    public class FileShared : SlackMessage
    {
        public string    file_id { get; set; }
        public SlackFile file    { get; set; }
    }

    [ SlackMessage( "file_unshared", apiType: Msg.RTM | Msg.Event, scope: "file:read" ) ]
    public class FileUnshared : SlackMessage
    {
        public string    file_id { get; set; }
        public SlackFile file    { get; set; }
    }
}

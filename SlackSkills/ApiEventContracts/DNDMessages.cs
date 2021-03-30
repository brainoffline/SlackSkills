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
    [ SlackMessage( "dnd_updated", apiType: Msg.RTM | Msg.Event, scope: "dnd:read" ) ]
    public class DNDUpdated : SlackMessage
    {
        public string    user       { get; set; }
        public DNDStatus dnd_status { get; set; }
    }

    [ SlackMessage( "dnd_updated_user", apiType: Msg.RTM | Msg.Event, scope: "dnd:read" ) ]
    public class DNDUpdatedUser : SlackMessage
    {
        public string    user       { get; set; }
        public DNDStatus dnd_status { get; set; }
    }
}

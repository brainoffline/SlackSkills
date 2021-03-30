using System;
using System.Collections.Generic;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

namespace SlackSkills.WebApiContracts
{
    [ SlackMessage( "dnd.endDnd",
                      apiType: Msg.UserToken,
                      scope: "dnd:write" ) ]
    public class DNDEndDnd { }

    [ SlackMessage( "dnd.endSnooze",
                      apiType: Msg.UserToken,
                      scope: "dnd:write" ) ]
    public class DNDEndSnooze { }

    [ SlackMessage( "dnd.info",
                      apiType: Msg.FormEncoded | Msg.UserToken,
                      scope: "dnd:read" ) ]
    public class DNDInfo : SlackMessage
    {
        
        public string? user { get; set; }
    }

    public class DNDInfoResponse : MessageResponse
    {
        public bool? dnd_enabled       { get; set; }
        public int?  next_dnd_start_ts { get; set; }
        public int?  next_dnd_end_ts   { get; set; }
        public bool? snooze_enabled    { get; set; }
        public int?  snooze_endtime    { get; set; }
        public int?  snooze_remaining  { get; set; }
    }

    [ SlackMessage( "dnd.setSnooze",
                      apiType: Msg.FormEncoded | Msg.UserToken,
                      scope: "dnd:write" ) ]
    public class DNDSetSnooze : SlackMessage
    {
        public int num_minutes { get; set; }
    }

    public class DNDSetSnoozeResponse : MessageResponse
    {
        public bool? snooze_enabled   { get; set; }
        public int?  snooze_endtime   { get; set; }
        public int?  snooze_remaining { get; set; }
    }

    [ SlackMessage( "dnd.teamInfo",
                      apiType: Msg.FormEncoded | Msg.BotToken | Msg.UserToken,
                      scope: "dnd:read" ) ]
    public class DNDTeamInfo : SlackMessage
    {
        
        public string[] users { get; set; }
    }

    public class DNDTeamInfoResponse : MessageResponse
    {
        public Dictionary< string, UserDNDInfo > users { get; set; }

        public class UserDNDInfo
        {
            public bool? dnd_enabled;
            public int?  next_dnd_start_ts { get; set; }
            public int?  next_dnd_end_ts   { get; set; }
        }
    }
}

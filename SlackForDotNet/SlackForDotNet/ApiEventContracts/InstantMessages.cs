﻿using System;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

namespace SlackForDotNet
{
    [ SlackMessage( "im_close", apiType: Msg.RTM | Msg.Event, scope: "im:read" ) ]
    public class ImClose : SlackMessage
    {
        public string user    { get; set; }
        public string channel { get; set; }
    }

    [ SlackMessage( "im_created", apiType: Msg.RTM | Msg.Event, scope: "im:read" ) ]
    public class ImCreated : SlackMessage
    {
        public string  user    { get; set; }
        public Channel channel { get; set; }
    }

    [ SlackMessage( "im_history_changed", apiType: Msg.RTM | Msg.Event, scope: "im:history" ) ]
    public class ImHistoryChanged : SlackMessage
    {
        public string latest   { get; set; }
        public string ts       { get; set; }
    }

    [ SlackMessage( "im_marked" ) ]
    public class ImMarked : SlackMessage
    {
        public string channel { get; set; }
        public string ts      { get; set; }
    }

    [ SlackMessage( "im_open", apiType: Msg.RTM | Msg.Event, scope: "im:read" ) ]
    public class ImOpen : SlackMessage
    {
        public string user    { get; set; }
        public string channel { get; set; }
    }

    [ SlackMessage( "mpim_close", scope: "im:read" ) ]
    public class MpimClose : SlackMessage
    {
        public string user    { get; set; }
        public string channel { get; set; }
    }

    [ SlackMessage( "mpim_history_changed", scope: "mpim:history" ) ]
    public class MpimHistoryChanged : SlackMessage
    {
        public string latest   { get; set; }
        public string ts       { get; set; }
        public int?   is_mpim  { get; set; }
    }

    [ SlackMessage( "mpim_joined", scope: "mpim:read" ) ]
    public class MpimJoined : SlackMessage
    {
        public Channel channel { get; set; }
    }

    [ SlackMessage( "mpim_open", scope: "im:read" ) ]
    public class MpimOpen : SlackMessage
    {
        public string user    { get; set; }
        public string channel { get; set; }
    }
}
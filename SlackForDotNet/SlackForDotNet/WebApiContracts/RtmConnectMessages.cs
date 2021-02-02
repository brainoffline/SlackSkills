using System;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

namespace SlackForDotNet.WebApiContracts
{
    [ SlackMessage( "rtm.connect" ) ]
    public class RtmConnectResponse : MessageResponse
    {
        public string url  { get; set; }
        public Self   self { get; set; }
        public Team   team { get; set; }
    }

    [ SlackMessage( "rtm.start" ) ]
    public class RtmStartResponse : MessageResponse
    {
        public string  accept_tos_url             { get; set; }
        public int?    cache_ts                   { get; set; }
        public string? cache_ts_version           { get; set; }
        public string  cache_version              { get; set; }
        public bool?   can_manage_shared_channels { get; set; }
        public string  latest_event_ts            { get; set; }

        public string url  { get; set; }
        public Self   self { get; set; }
        public Team   team { get; set; }

        public User[]                     users    { get; set; }
        public Channel[]                  channels { get; set; }
        public Group[]                    groups   { get; set; }
        public MultiPartyInstantMessage[] mpims    { get; set; }
        public InstantMessage[]           ims      { get; set; }
        public Bot[]                      bots     { get; set; }
    }
}

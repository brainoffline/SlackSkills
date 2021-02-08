#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

using System;
using System.Collections.Generic;

namespace SlackForDotNet
{
    [ SlackMessage( "events_api" ) ]
    public class EventCallback : Envelope< CallbackMessage > { }

    public class CallbackMessage : SlackMessage
    {
        public string               token                 { get; set; }
        public string               team_id               { get; set; }
        public string               api_app_id            { get; set; }
        public SlackMessage?        @event                { get; set; }
        public string               event_id              { get; set; }
        public int                  event_time            { get; set; }
        public List<Authorization>? authorisations        { get; set; }
        public bool                 is_ext_shared_channel { get; set; }
    }
}

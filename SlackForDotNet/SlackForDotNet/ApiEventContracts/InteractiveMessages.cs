// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

using System.Collections.Generic;

namespace SlackForDotNet
{
    [ SlackMessage( "interactive" ) ]
    public class Interactive : Envelope< SlackMessage >, IEnvelope<SlackMessage>
    {

    }

    [SlackMessage( "shortcut")]
    public class GlobalShortcut : SlackMessage
    {
        public string?      token                 { get; set; }
        public string?      action_ts             { get; set; }
        public Team?        team                  { get; set; }
        public UserMinimal? user                  { get; set; }
        public bool?        is_enterprise_install { get; set; }
        public Enterprise?  enterprise            { get; set; }
        public string?      callback_id           { get; set; }
        public string?      trigger_id            { get; set; }
    }

    [SlackMessage( "message_action")]
    public class MessageShortcut : SlackMessage
    {
        public string?      token                 { get; set; }
        public string?      action_ts             { get; set; }
        public string?      message_ts            { get; set; }
        public Channel?     channel               { get; set; }
        public Team?        team                  { get; set; }
        public UserMinimal? user                  { get; set; }
        public bool?        is_enterprise_install { get; set; }
        public Enterprise?  enterprise            { get; set; }
        public string?      callback_id           { get; set; }
        public string?      trigger_id            { get; set; }
        public string?      response_url          { get; set; }
        public Message?     message               { get; set; }
    }

    [SlackMessage( "view_submission" ) ]
    public class ViewSubmission : SlackMessage
    {
        public Team?              team                  { get; set; }
        public UserMinimal?       user                  { get; set; }
        public string?            token                 { get; set; }
        public string?            trigger_id            { get; set; } 
        public View?              view                  { get; set; }
        public string             hash                  { get; set; }
        public List<ResponseUrl>? response_urls         { get; set; }
        public bool?              is_enterprise_install { get; set; }
        public Enterprise?        enterprise            { get; set; }
    }
    
    [SlackMessage( "view_closed")]
    public class ViewClosed : SlackMessage
    {
        public Team?        team       { get; set; }
        public UserMinimal? user       { get; set; }
        public View?        view       { get; set; }
        public bool?        is_cleared { get; set; }
    }
}

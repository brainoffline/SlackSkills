using System;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

namespace SlackForDotNet
{
    /// <summary>
    /// Respond with
    /// https://api.slack.com/surfaces/modals/using#updating_response
    /// or
    /// https://api.slack.com/surfaces/modals/using#pushing_views
    /// or
    /// https://api.slack.com/surfaces/modals/using#closing_views
    /// </summary>
    [SlackMessage( "block_actions" ) ]
    public class BlockAction : SlackMessage
    {
        public Team           team                  { get; set; }
        public User           user                  { get; set; }
        public string         api_app_id            { get; set; }
        public string         token                 { get; set; }
        public Container?     container             { get; set; }
        public Enterprise?    enterprise            { get; set; }
        public bool?          is_enterprise_install { get; set; }
        public View.State?    state                 { get; set; }
        public string?        trigger_id            { get; set; }
        public Channel?       channel               { get; set; }
        public MessageBase?   message               { get; set; }
        public string?        response_url          { get; set; }
        public ResponseUrl[]? response_urls         { get; set; }
        public SlackAction[]? actions               { get; set; }
    }

    public class ResponseAction : SlackMessage
    {
        public string response_action { get; set; } // options (update, push, clear)
        public View   view            { get; set; }
    }

}

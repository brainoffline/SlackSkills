using System;
using System.Collections.Generic;
using System.Net;

using SlackSkills.WebApiContracts;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

namespace SlackSkills
{
    [ SlackMessage( "app_home_opened", apiType: Msg.Event ) ]
    public class AppHomeOpened : SlackMessage
    {
        public string  user     { get; set; }
        public string  channel  { get; set; }
        public string  tab      { get; set; }
        public View?   view     { get; set; }
        public string? event_ts { get; set; }
    }

    [ SlackMessage( "app_mention", apiType: Msg.Event, scope: "app_mentions:read" ) ]
    public class AppMention : SlackMessage
    {
        public string user    { get; set; }
        public string text    { get; set; }
        public string ts      { get; set; }
        public string channel { get; set; }
    }

    /// <summary>
    /// Indicates your app's event subscriptions are being rate limited
    /// This Events API-only event type has no "inner event".
    /// </summary>
    [ SlackMessage( "app_rate_limited", apiType: Msg.Event ) ]
    public class AppRateLimited : SlackMessage
    {
        public string token               { get; set; }
        public string team_id             { get; set; }
        public string minute_rate_limited { get; set; }
        public string api_app_id          { get; set; }
    }

    [ SlackMessage( "app_requested", apiType: Msg.Event ) ]
    public class AppRequested : SlackMessage
    {
        public int?       date_created        { get; set; }
        public AppInfo    app_request         { get; set; }
        public Resolution previous_resolution { get; set; }
        public User       user                { get; set; }
        public Team       team                { get; set; }
        public Scopes[]   scopes              { get; set; }
        public string     message             { get; set; }
    }

    [ SlackMessage( "app_uninstalled", apiType: Msg.Event ) ]
    public class AppUninstalled : SlackMessage { }

    [ SlackMessage( "email_domain_changed", apiType: Msg.RTM | Msg.Event, scope: "team:read" ) ]
    public class EmailDomainChanged : SlackMessage
    {
        public string email_domain { get; set; }
    }

    [ SlackMessage( "goodbye" ) ]
    public class Goodbye : SlackMessage { }

    [ SlackMessage( "invite_requested", apiType: Msg.Event, scope: "admin.invites:read" ) ]
    public class InviteRequested : SlackMessage
    {
        public InviteRequest invite_request { get; set; }
    }

    [ SlackMessage( "url_verification", apiType: Msg.Event ) ]
    public class UrlVerification : SlackMessage
    {
        public string token     { get; set; }
        public string challenge { get; set; }
    }

    public class UrlVerificationResponse : SlackMessage
    {
        public string challenge { get; set; }
    }

}

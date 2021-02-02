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
    [Obsolete]
    [ SlackMessage( "oauth.access" ) ]
    public class OAuthAccess : SlackMessage
    {
        public string client_id      { get; set; }
        public string client_secret  { get; set; }
        public string code           { get; set; }
        public string redirect_uri   { get; set; }
        public string single_channel { get; set; }
    }

    [Obsolete]
    public class OAuthAccessResponse : MessageResponse
    {
        public string          access_token     { get; set; }
        public string          scope            { get; set; }
        public string          team_name        { get; set; }
        public string          team_id          { get; set; }
        public string          enterprise_id    { get; set; }
        public Bot             bot              { get; set; }
        public IncomingWebhook incoming_webhook { get; set; }
    }
    
    [SlackMessage( "oauth.v2.access", apiType: Msg.FormEncoded)]
    public class OAuthAccess2 : SlackMessage
    {
        public string client_id     { get; set; }
        public string client_secret { get; set; }
        public string code          { get; set; }
        public string redirect_uri  { get; set; }
    }
    
    public class OAuthAccessResponse2 : MessageResponse
    {
        public string           access_token          { get; set; }
        public string           token_type            { get; set; }
        public string           scope                 { get; set; }
        public string           bot_user_id           { get; set; }
        public string           app_id                { get; set; }
        public Team             team                  { get; set; }
        public bool             is_enterprise_install { get; set; }
        public Enterprise?      enterprise            { get; set; }
        public IncomingWebhook? incoming_webhook      { get; set; }
        public AuthedUser?      authed_user           { get; set; }
    }


    [SlackMessage( "auth.revoke", apiType: Msg.FormEncoded ) ]
    public class AuthRevoke : SlackMessage
    {
        public bool? test { get; set; }
    }

    [ SlackMessage( "auth.test", apiType: Msg.FormEncoded ) ]
    public class AuthTest : SlackMessage { }

    [ SlackMessage( "auth.test" ) ]
    public class AuthTestResponse : MessageResponse
    {
        public string url     { get; set; }
        public string team    { get; set; }
        public string user    { get; set; }
        public string team_id { get; set; }
        public string user_id { get; set; }
    }
}

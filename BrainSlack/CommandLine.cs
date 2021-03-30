using System;

using Asciis;

namespace BrainSlack
{
    [Command( "BrainSlack", Description = "Slack CLI using SlackSkills")]
    class CommandLine
    {
        [Param( "i|Id|ClientId", "Slack Client Id. Mandatory")]
        public string? ClientId { get; set; }

        [Param( "s|Secret|ClientSecret", "Slack Client Secret. Mandatory")]
        public string? ClientSecret { get; set; }

        [Param( "a|AppLevel|AppLevelToken", "App Level Token. Required for SocketMode")]
        public string? AppLevelToken { get; set; }

        [Param( "b|Bot|BotAccessToken", "Bot access token. If not supplied, browser will be launched to login")]
        public string? BotAccessToken { get; set; }

        [Param( "u|User|UserAccessToken", "User access token. If user scopes are supplied and no access token, browser will be launched to login")]
        public string? UserAccessToken { get; set; }

        [Param( "bs|BotScopes", "Bot Scopes (comma separated). Required if no Bot access token supplied")]
        public string? BotScopes { get; set; }

        [Param( "us|UserScopes", "User Scopes (comma separated). Indicates a user access token is required")]
        public string? UserScopes { get; set; }

        [Param( "r|Redirect|RedirectUrl", "Url used to host browser calls during login (OAuth 2.0 security)")]
        public string? RedirectUrl { get; set; }
    }
}

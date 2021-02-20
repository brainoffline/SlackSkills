// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

using System;
using System.Collections.Generic;

using SlackForDotNet.Surface;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

namespace SlackForDotNet
{
    /*
        {
          "envelope_id": "7ed8bc9d-e133-4638-b971-50c3d57a2afa",
          "payload": {
            "token": "F9XypcTGSQAy4kBY9e3kW3iX",
            "team_id": "T02C5T45G",
            "team_domain": "brainoffline",
            "channel_id": "DPR9Q37PS",
            "channel_name": "directmessage",
            "user_id": "U02C5T45J",
            "user_name": "brainoffline",
            "command": "/slackdk",
            "text": "",
            "api_app_id": "APTE2Q3CN",
            "is_enterprise_install": "false",
            "response_url": "https://hooks.slack.com/commands/T02C5T45G/1736895681301/X6s2WMfSWxv1HMtOWM1O9rt2",
            "trigger_id": "1752610767569.2413922186.b36e826a03e8d65b127ce57db6e2a818"
          },
          "type": "slash_commands",
          "accepts_response_payload": true
        }
     */

    /// <summary>
    ///     Slash Command 
    /// </summary>
    /// <remarks>
    ///     https://api.slack.com/interactivity/slash-commands
    ///     Wrapped in a <see cref="SlashCommandsEnvelope">SlashCommandsEnvelope</see>/>
    /// </remarks>
    /// <returns>
    ///     Should acknowledge with a <see cref="SlashCommandResponse">SlashCommandResponse</see> object.
    /// </returns>
    /// <example>
    /// {
    ///     "envelope_id": "7ed8bc9d-e133-4638-b971-50c3d57a2afa",
    ///     "payload":
    ///     {
    ///         "token": "Fblah",
    ///         "team_id": "Tblah",
    ///         "team_domain": "brainoffline",
    ///         "channel_id": "Dblah",
    ///         "channel_name": "directmessage",
    ///         "user_id": "Ublah",
    ///         "user_name": "brainoffline",
    ///         "command": "/blah",
    ///         "text": "",
    ///         "api_app_id": "Ablah",
    ///         "is_enterprise_install": "false",
    ///         "response_url": "https://hooks.slack.com/commands/blah/blah/blah",
    ///         "trigger_id": "1752610767569.2413922186.blah"
    ///     },
    ///     "type": "slash_commands",
    ///     "accepts_response_payload": true
    /// }
    /// </example>
    [SlackMessage("slash_command")]
    public class SlashCommand : SlackMessage
    {
        public SlashCommand()
        {
            type = "slash_command";
        }
        
        public string token        { get; set; }
        public string team_id      { get; set; }
        public string team_domain  { get; set; }
        public string channel_id   { get; set; }
        public string channel_name { get; set; }
        public string user_id      { get; set; }
        public string user_name    { get; set; }
        public string command      { get; set; }
        public string text         { get; set; }
        public string response_url { get; set; }
        public string trigger_id   { get; set; }
    }

    public class SlashCommandResponse
    {
        public string        response_type    { get; set; } = "in_channel"; // can be `ephemeral`
        public bool?         delete_original  { get; set; }
        public bool?         replace_original { get; set; }
        public string?       text             { get; set; }
        public List<Layout>? blocks           { get; set; }

        public SlashCommandResponse Add(Layout block)
        {
            blocks ??= new List<Layout>();
            blocks.Add(block);

            return this;
        }

    }

    [SlackMessage( "slash_commands")]
    public class SlashCommandsEnvelope : Envelope<SlashCommand> { }
}

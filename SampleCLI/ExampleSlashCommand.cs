using System;
using System.Collections.Generic;

using Asciis;

using SlackForDotNet;
using SlackForDotNet.Surface;

namespace SampleCLI
{
    [Command("/slashdk", Description = "Slash Command Example")]
    internal class ExampleSlashCommand : SlackSlashCommand
    {
        [ Param( "c|Comment", "A comment to display" ) ]
        public string? Comment { get; set; }

        public override void OnCommand( List< string > args )
        {
            if (string.IsNullOrWhiteSpace(Comment))
            {
                var response = new SlashCommandResponse
                               {
                                   response_type = "ephemeral", // visible to only the user
                                   text          = new ParamParser<ExampleSlashCommand>().Help()
                               };
                SlackApp!.Push(new AcknowledgeResponse<SlashCommandResponse>
                                  (Envelope!.envelope_id, response));
            }
            else
            {
                var response = new SlashCommandResponse()
                              .Add(new HeaderLayout("Slash command response"))
                              .Add(new SectionLayout { block_id = "wave_id", text = $":wave: {Comment}" })
                              .Add(new SectionLayout
                                   {
                                       block_id  = "button-section",
                                       text      = "*Welcome*",
                                       accessory = new ButtonElement { text = "Just a button", action_id = "just-a-button" }
                                   });
                SlackApp!.Push(new AcknowledgeResponse<SlashCommandResponse>(Envelope!.envelope_id, response));
            }

        }
    }
}

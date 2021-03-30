using System;
using System.Collections.Generic;

using Asciis;

using SlackSkills;
using SlackSkills.Surface;

namespace BrainSlack
{
    [Command( "/estimate", Description = "Estimate story")]
    internal class EstimateSlashCommand : SlackSlashCommand
    {
        private EstimateSurface _estimateSurface;
        private SlackSurface    _revealSurface;
        private EstimatorDialog _estimatorDialog;

        public override void OnCommand( List< string > args )
        {
            _estimatorDialog = new EstimatorDialog( SlackApp!, Message!.text )
                               {
                                   Submitted = OnDialogSubmitted
                               };

            SlackApp!.OpenModalSurface( _estimatorDialog, Message!.trigger_id );
        }

        private void OnDialogSubmitted( ViewSubmission msg )
        {
            // Send the publicly visible surface to channel
            _estimateSurface = new EstimateSurface( SlackApp! )
                              {
                                  Title       = _estimatorDialog.EstimateTitle!,
                                  Description = _estimatorDialog.EstimateDescription,
                                  Scale       = _estimatorDialog.ScaleSelect.selected_option?.value,
                                  Risk        = _estimatorDialog.RiskSelect.selected_option?.value
                              };
            SlackApp!.OpenMessageSurface( _estimateSurface, Message!.channel_id );

            // Send a message surface that is visible only to the Estimator
            _revealSurface = new SlackSurface(SlackApp) { Title = "Reveal" }
               .Add(new SectionLayout("estimate-reveal")
                    {
                        text = "Reveal all estimations to the team",
                        accessory = new ButtonElement("estimate-reveal-button", "Reveal")
                                    {
                                        Clicked = OnReveal,
                                        value   = "reveal"
                                    }
                    });
            SlackApp!.OpenMessageSurface(_revealSurface, Message!.channel_id, Message.user_id);
        }

        private void OnReveal(SlackSurface surface, ButtonElement button, BlockActions actions)
        {
            // Remove the Reveal messages
            // For ephemeral messages, they can only be modified using the response_url
            var response = new SlashCommandResponse
                           {
                               response_type = "ephemeral", // visible to only the user
                               replace_original = true,
                               delete_original = true,
                               text = ""
                           };
            SlackApp!.Respond( actions.response_url, response );

            // Change the estimate surface to reveal all the results
            _estimateSurface.Reveal();
        }
    }
}

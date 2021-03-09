using System;
using System.Collections.Generic;
using System.Diagnostics;

using Asciis;

using JetBrains.Annotations;

using SlackForDotNet;
using SlackForDotNet.Surface;
using SlackForDotNet.WebApiContracts;

namespace BrainSlack
{
    [Command( "/estimate", Description = "Estimate story")]
    internal class EstimateSlashCommand : SlackSlashCommand
    {
        private InputLayout      _titleLayout;
        private InputLayout      _descriptionLayout;
        private TextInputElement _titleInput;
        private TextInputElement _descriptionInput;

        private SlackSurface _modalSurface;
        private SlackSurface _messageSurface;
        private SlackSurface _revealSurface;

        public override void OnCommand( List< string > args )
        {
            _titleInput = new TextInputElement("estimate-title")
                          {
                              placeholder   = "enter the title of the story",
                              initial_value = Message!.text,
                              //min_length    = 1,
                              max_length    = 30
                          };
            _descriptionInput = new TextInputElement("estimate-description")
                                {
                                    multiline = true,
                                };

            _titleLayout       = new InputLayout( "estimate-title",       "Title",       _titleInput );
            _descriptionLayout = new InputLayout( "estimate-description", "Description", _descriptionInput ) { optional = true };

            _modalSurface = new SlackSurface(SlackApp!)
                       {
                           Title     = "Estimate Story",
                           Submitted = OnSubmitted,
                           Closed    = OnClosed
                       }
                      .Add( _titleLayout  )
                      .Add( _descriptionLayout );

            SlackApp!.OpenModal( _modalSurface, Message!.trigger_id );
        }

        private void OnSubmitted( ViewSubmission msg )
        {
            if (string.IsNullOrEmpty( _titleInput.value ))
            {
                _titleLayout.hint = "Title is a mandatory field";
                SlackApp!.OpenModal(_modalSurface, msg.trigger_id!);
            }
            else
            {
                _revealSurface = new SlackSurface( SlackApp ) { Title = "Reveal" }
                   .Add( new SectionLayout( "estimate-reveal" )
                         {
                             text      = "Reveal all estimations to the team",
                             accessory = new ButtonElement( "estimate-reveal-button", "Reveal" )
                                         {
                                             Clicked = OnReveal, value = "reveal"
                                         }
                         } );
                SlackApp!.OpenSurface( _revealSurface, Message!.channel_id, Message.user_id );

                _messageSurface = new EstimateChannelSurface( SlackApp! );
                SlackApp!.OpenSurface( _messageSurface, Message!.channel_id );
            }
        }

        private void OnClosed( ViewClosed msg )
        {
            // Only visible by the original user
            SlackApp!.Say( "Estimation cancelled", Message!.channel_id, Message.user_id );
        }

        private void OnReveal(SlackSurface surface, ButtonElement button, BlockActions actions)
        {
            SlackApp!.RemoveSurface( _revealSurface );
            SlackApp!.RemoveSurface( _messageSurface );

            SlackApp!.Say("Estimation revealed", Message!.channel_id);
        }
    }
}

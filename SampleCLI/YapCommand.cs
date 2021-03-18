using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Asciis;

using SlackForDotNet;
using SlackForDotNet.Surface;
using SlackForDotNet.WebApiContracts;

namespace SampleCLI
{
    [Command("Yap", Description = "Yappity yap yap")]
    public class YapMessageCommand : SlackMessageCommand
    {
        [Param("o|options", "Yap options (Message)")]
        public YapOptions Options { get; set; }

        public override void OnCommand( List< string > extras )
        {
            if (Message == null) return;

            switch (Options)
            {
                case YapOptions.NotSet:
                    var message = "Choose your options first\n```\n" + new ParamParser<YapMessageCommand>().Help() + "\n```";
                    SlackApp!.Say( message, Message.channel, Message.user );
                    break;
                case YapOptions.Message:
                    SendMessage();
                    break;
                case YapOptions.Dialog:
                    DialogMessage();
                    break;
                default: 
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SendMessage()
        {
            if (Message == null) return;

            var surface = new SlackSurface( SlackApp! )
                            {
                                Title = "Yappity yap!"
                            }
                         .Add( new HeaderLayout( "Yappity yap!" ) )
                         .Add( new SectionLayout
                               {
                                   block_id = "yappity-yap-id",
                                   text     = "Don't talk back",
                                   accessory = new ButtonElement( "dont-talk-back", "ok" )
                                               {
                                                   Clicked = ( slackSurface, button, actions ) =>
                                                             {
                                                                 TalkBack();
                                                             }
                                               }
                               } );
            SlackApp!.OpenMessageSurface( surface, Message.channel );
        }

        private void TalkBack()
        {
            if (Message == null) return;

            SlackApp!.Say( "What cha gonna do bout it!", channel: Message.channel );
        }

        private void DialogMessage()
        {
            if (Message == null) return;

            var surface = new SlackSurface( SlackApp! )
                          {
                              Title = "Yappity yap!",
                          }
                         .Add( new HeaderLayout( "Yappity yap!" ) )
                         .Add( new SectionLayout
                               {
                                   block_id  = "yappity-yap-id",
                                   text      = "Don't press the button",
                                   accessory = new ButtonElement( "dont-talk-back", "ok" )
                                               {
                                                   Clicked = ( slackSurface, button, action ) => OpenModal( action.trigger_id! )
                                               }
                               } );
            SlackApp!.OpenMessageSurface(surface, Message.channel);
        }

        private async void OpenModal( string triggerId )
        {
            TextInputElement? tie     = null;
            var               surface = new SlackSurface( SlackApp! )
                                        {
                                            Title = "Yappity Yapp Modal"
                                        }
               .Add(new InputLayout("yap-block-id",
                                    "Useful information goes here",
                                    tie = (new TextInputElement { action_id = "useful-id" })));

            await SlackApp!.OpenModalSurface( surface, triggerId );
            if (surface != null)
                surface.Submitted = Submitted;

            void Submitted(ViewSubmission vs)
            {
                SlackApp!.Say($"{tie!.value}", channel: Message!.channel);
            }
        }
    }

    public enum YapOptions
    {
        NotSet,
        Message,
        Dialog
    }
}

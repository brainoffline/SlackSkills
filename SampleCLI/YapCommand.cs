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
                    SlackApp!.Say( "Choose your options first", Message.channel, Message.user );
                    break;
                case YapOptions.Message:
                    SendMessage();
                    break;
                case YapOptions.Dialog:
                    InlineMessage();
                    break;
            }
        }

        private void SendMessage()
        {
            if (Message == null) return;

            var surface = new SlackSurface( SlackApp! )
                            {
                                CallbackId = "yap-callback-id",
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
            SlackApp!.OpenSurface( surface, Message.channel );
        }

        private void TalkBack()
        {
            if (Message == null) return;

            SlackApp!.Say( "What cha gonna do bout it!", channel: Message.channel );
        }

        private void InlineMessage()
        {
            if (Message == null) return;

            var surface = new SlackSurface( SlackApp! )
                          {
                              CallbackId = "yap-callback-id",
                              Title = "Yappity yap!"
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
            SlackApp!.OpenSurface(surface, Message.channel);
        }

        private void OpenModal( string triggerId )
        {
            var view = (ModalView)new ModalView { title = "Yappity Yapp Modal", submit = "Do It!" }
               .Add( new InputLayout( "Useful information goes here",
                                     new TextInputElement { action_id = "useful-id" } ) );

            SlackApp!.OpenModal( view, triggerId );
        }
    }

    public enum YapOptions
    {
        NotSet,
        Message,
        Dialog
    }
}

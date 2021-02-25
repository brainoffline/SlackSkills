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
    public class YapCommand : BaseSlackCommand
    {
        [Param("o|options", "Yap options (Message)")]
        public YapOptions Options { get; set; }

        public override void OnCommand( List< string > extras )
        {
            switch (Options)
            {
                case YapOptions.NotSet:
                    SlackApp!.Say( "Choose your options first", Message.channel, Message.user );
                    break;
                case YapOptions.Message:
                    SendMessage();
                    break;
                case YapOptions.Dialog:
                    SendDialog();
                    break;
            }
        }

        private void SendMessage()
        {
            if (!string.IsNullOrEmpty( Message.thread_ts ))
            {
                var surface = ( new InlineMessageSurface( SlackApp!, "Yappity yap!" ) { CallbackId = "yap-callback-id" } )
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
                SlackApp!.OpenSurface( surface, Message );
            }
            else
                SlackApp!.Say( new List<Layout>()
                              {
                                new HeaderLayout("Yappity yap!")
                              }, Message.channel );
        }

        private void TalkBack()
        {
            SlackApp!.Say( "What cha gonna do bout it!", channel: Message.channel );
        }

        private void SendDialog()
        {
            var surface = new InlineMessageSurface( SlackApp!, "Yappity yap!" )
                          {
                              CallbackId = "yap-callback-id"
                          }
                         .Add( new HeaderLayout( "Yappity yap!" ) )
                         .Add( new SectionLayout
                               {
                                   block_id  = "yappity-yap-id",
                                   text      = "Don't press the button",
                                   accessory = new ButtonElement( "dont-talk-back", "ok" )
                                               {
                                                   Clicked = ( slackSurface, button, actions ) => OpenDialog( slackSurface )
                                               }
                               } );
            SlackApp!.OpenSurface(surface, Message);
        }

        private void OpenDialog( SlackSurface slackSurface )
        {
            var view = (ModalView)new ModalView { title = "Yappity Yapp Modal", submit = "Do It!" }
               .Add( new InputLayout( "Useful information goes here", new TextInputElement { action_id = "useful-id" } ) );

            var surface = new DialogSurface( SlackApp!, view );

            SlackApp!.OpenSurface( surface, Message );
        }
    }

    public enum YapOptions
    {
        NotSet,
        Message,
        Dialog
    }
}

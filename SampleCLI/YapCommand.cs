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
            }
        }

        private void SendMessage()
        {
            if (!string.IsNullOrEmpty( Message.thread_ts ))
            {
                var surface = new InlineMessageSurface( SlackApp!,
                                                       "Yappity yap!",
                                                       new List< Layout >()
                                                       {
                                                           new HeaderLayout( "Yappity yap!" ),
                                                           new SectionLayout
                                                           {
                                                               block_id = "yappity-yap-id",
                                                               text = "Don't talk back",
                                                               accessory = new ButtonElement( "dont-talk-back", "ok" )
                                                                           {
                                                                               Clicked = ( s, e ) =>
                                                                                         {
                                                                                             TalkBack();
                                                                                         }
                                                                           }
                                                           }
                                                       }
                                                      ) { CallbackId = "yap-callback-id" };
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
    }

    public enum YapOptions
    {
        NotSet,
        Message
    }
}

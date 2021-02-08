using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using JetBrains.Annotations;

using SlackForDotNet;
using SlackForDotNet.Surface;
using SlackForDotNet.WebApiContracts;

namespace SampleCLI
{
    public partial class Program
    {
        private HametabSurface? _home;
        
        public void SetupSurface()
        {
            Console.WriteLine( "Slack Surface CLI" );

            _app.OnMessage<AppHomeOpened>(OnHomeOpenedSurface);
            _app.OnMessage<SlashCommands>(OnSlashCommandSurface);
        }

        private async void OnHomeOpenedSurface( ISlackApp slackApp, AppHomeOpened msg )
        {
            _home ??= new HametabSurface( _app );

            await _app.PublishHomepage( _home, msg );
        }

        private void OnSlashCommandSurface(ISlackApp slackApp, SlashCommands msg)
        {
            
        }
    }



    public class HametabSurface : SlackSurface
    {
        private SectionLayout _statusLayout;

        public HametabSurface( [ NotNull ] ISlackApp slackApp ) : base( slackApp )
        {
            Title = "SlackDK Title";
            Layouts = new List<Layout>
            {
                new HeaderLayout( "SlackDK Home Page" ),
                new SectionLayout { text = new Markdown( "*Welcome* to the _new_ and _improved_ SlackDK ~inner-city apartment~ home page" ) },
                
                (_statusLayout = new SectionLayout { text = ":wave: hello" }),
                
                new SectionLayout
                     {
                         text      = "It's a kind of",
                         block_id  = "magic-block",
                         accessory = new ButtonElement
                                     {
                                         action_id = "magic-button", 
                                         text      = "Magic",
                                         Clicked = ( sender, args ) =>
                                                   {
                                                       _statusLayout.text = ":banana: :sob: Magic is hard";
                                                       slackApp.Update( this );
                                                   }
                                     }
                     },
                new InputLayout
                {
                    label = "What are you doing today", 
                    element = new TextInputElement("what-doing")
                    {
                        TextUpdated = ( s, value ) =>
                                      {
                                          _statusLayout.text =
                                              new Markdown( $"Keep talking brah. I'm listening. `{value}`" );
                                          slackApp.Update( this );
                                      },
                        dispatch_action_config = DispatchAction.CharacterEntered
                    },
                    dispatch_action = true
                }
            };
        }
    }
}

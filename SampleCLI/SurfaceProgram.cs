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
            Layouts = new List< Layout >
                      {
                          new HeaderLayout( "SlackDK Home Page" ),
                          new SectionLayout { text = new Markdown( "*Welcome* to the _new_ and _improved_ SlackDK ~inner-city apartment~ home page" ) },
                          ( _statusLayout = new SectionLayout { text = ":wave: hello" } ),
                          new SectionLayout
                          {
                              text     = "It's a kind of",
                              block_id = "magic-block",
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
                              block_id = "textinput-block",
                              label    = "What are you doing today",
                              element = new TextInputElement( "what-doing" )
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
                          },
                          new InputLayout
                          {
                              block_id = "checkboxes-block",
                              label    = "Update the following survey",
                              element = new CheckboxesElement( "what-survey" )
                                        {
                                            options = new List< Option >
                                                      {
                                                          new Option( "I love the historical sound of dialup",           "dialup" ),
                                                          new Option( "You can never get enough data",                   "enough-data" ),
                                                          new Option( "73.6% of statistics are true",                    "lies-statistics" ),
                                                          new Option( "The future is soo bright, I have to wear shades", "future-shades" ),
                                                      }
                                        }
                          },
                          new InputLayout
                          {
                              block_id = "options-block",
                              label    = "Update the following option",
                              element = new RadioButtonsElement( "what-options" )
                                        {
                                            options = new List< Option >
                                                      {
                                                          new Option( "I love fake news",            "fake-news" ),
                                                          new Option( "Enough data is never enough", "more-data" ),
                                                          new Option( "I trust 73.6% of statistics", "true-lies" ),
                                                          new Option( "The future is yesterday",     "vision" ),
                                                      }
                                        }
                          },
                          new InputLayout
                          {
                              block_id = "date-block",
                              label = "What date will it happen",
                              element = new DatePickerElement("what-date")
                          },
                          // still in beta
                          //new InputLayout
                          //{
                          //    label   = "What time will it happen",
                          //    element = new TimePickerElement("what-time")
                          //},
                          new SectionLayout
                          {
                              block_id = "overflow-block",
                              text = new Markdown("Whatever you do, :x: do not select a menu item"),
                              accessory = new OverflowMenuElement("what-overflow")
                              {
                                  action_id = "what-overflow",
                                  options = new List< Option >
                                            {
                                                new Option( "Erase Everything", "over-erase" ),
                                                new Option( "Delete something", "over-delete" ),
                                                new Option( "Update whatever", "over-update" ),
                                            },
                                  Clicked = ( s, id ) =>
                                            {
                                                _statusLayout.text =
                                                    new Markdown( $"I told ya bro. I'm listening. `{id}`" );
                                                slackApp.Update( this );
                                            }
                              }
                          },
                          new SectionLayout
                          {
                              block_id = "select-block",
                              text     = new Markdown("Fixed list for selection"),
                              accessory = new SlackForDotNet.Surface.SelectElement
                                          {
                                              action_id = "what-selection",
                                              options = new List< Option >
                                                        {
                                                            new Option( "One",   "select-one" ),
                                                            new Option( "Two",   "select-two" ),
                                                            new Option( "Three", "select-three" ),
                                                            new Option( "Four", "select-four" ),
                                                            new Option( "Five", "select-five" ),
                                                        },
                                              Clicked = ( s, id ) =>
                                                        {
                                                            _statusLayout.text =
                                                                new Markdown( $"Fixed selection is the bomb. `{id}`" );
                                                            slackApp.Update( this );
                                                        }
                                          }
                          },
                          new SectionLayout
                          {
                              block_id = "select-external-block",
                              text     = "Dynamic list for selection",
                              accessory = new SelectExternalElement
                                          {
                                              action_id = "what-select-dynamic",
                                              placeholder = "The start of something dynamic",
                                              Suggestions = value => new List< Option >
                                                                     {
                                                                         new ( value + " One",   value + "-select-one" ),
                                                                         new ( value + " Two",   value + "-select-two" ),
                                                                         new ( value + " Three", value + "-select-three" ),
                                                                         new ( value + " Four",  value + "-select-four" ),
                                                                         new ( value + " Five",  value + "-select-five" ),
                                                                     },
                                              Clicked = ( s, id ) =>
                                                        {
                                                            _statusLayout.text =
                                                                new Markdown( $"Dynamic selection worked. Wow. `{id}`" );
                                                            slackApp.Update( this );
                                                        }
                                          }
                          }
                      };
        }
    }
}

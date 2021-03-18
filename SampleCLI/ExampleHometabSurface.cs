using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using SlackForDotNet;
using SlackForDotNet.Surface;

namespace SampleCLI
{
    public class ExampleHometabSurface : SlackSurface
    {
        private readonly SectionLayout _statusLayout;

        public ExampleHometabSurface( [ NotNull ] ISlackApp slackApp ) : base( slackApp )
        {
            Title = "SlackDK Title";
            var layouts = new List< Layout >
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
                                              Clicked = ( ( surface, button, actions ) => 
                                                          { 
                                                              _statusLayout.text = ":banana: :sob: Magic is hard";
                                                              slackApp.Update( this );
                                                          } ) 
                                          }
                          },
                          new InputLayout
                          {
                              block_id = "textinput-block",
                              label    = "What are you doing today",
                              element = new TextInputElement( "what-doing" )
                                        {
                                            TextUpdated = ( s, _, a ) =>
                                                          {
                                                              _statusLayout.text =
                                                                  new Markdown( $"Keep talking brah. I'm listening. `{a.value ?? ""}`" );
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
                          new InputLayout { block_id = "date-block", label = "What date will it happen", element = new DatePickerElement( "what-date" ) },
                          // still in beta
                          //new InputLayout
                          //{
                          //    label   = "What time will it happen",
                          //    element = new TimePickerElement("what-time")
                          //},
                          new SectionLayout
                          {
                              block_id = "overflow-block",
                              text     = new Markdown( "Whatever you do, :x: do not select a menu item" ),
                              accessory = new OverflowMenuElement( "what-overflow" )
                                          {
                                              action_id = "what-overflow",
                                              options = new List< Option >
                                                        {
                                                            new Option( "Erase Everything", "over-erase" ),
                                                            new Option( "Delete something", "over-delete" ),
                                                            new Option( "Update whatever",  "over-update" ),
                                                        },
                                              Clicked = ( s, _, a ) =>
                                                        {
                                                            _statusLayout.text =
                                                                new Markdown( $"I told ya bro. I'm listening. `{a.selected_option}`" );
                                                            slackApp.Update( this );
                                                        }
                                          }
                          },
                          new SectionLayout
                          {
                              block_id = "select-block",
                              text     = new Markdown( "Fixed list for selection" ),
                              accessory = new SlackForDotNet.Surface.SelectElement
                                          {
                                              action_id = "what-selection",
                                              options = new List< Option >
                                                        {
                                                            new Option( "One",   "select-one" ),
                                                            new Option( "Two",   "select-two" ),
                                                            new Option( "Three", "select-three" ),
                                                            new Option( "Four",  "select-four" ),
                                                            new Option( "Five",  "select-five" ),
                                                        },
                                              Clicked = ( s, _ , a ) =>
                                                        {
                                                            _statusLayout.text =
                                                                new Markdown( $"Fixed selection is the bomb. `{a.selected_option}`" );
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
                                              action_id   = "what-select-dynamic",
                                              placeholder = "The start of something dynamic",
                                              Suggestions = value =>
                                                                new Options
                                                                {
                                                                    options = new List< Option >
                                                                              {
                                                                                  new( value + " One", value   + "-select-one" ),
                                                                                  new( value + " Two", value   + "-select-two" ),
                                                                                  new( value + " Three", value + "-select-three" ),
                                                                                  new( value + " Four", value  + "-select-four" ),
                                                                                  new( value + " Five", value  + "-select-five" ),
                                                                              }
                                                                },
                                              Clicked = ( s, _, a ) =>
                                                        {
                                                            _statusLayout.text =
                                                                new Markdown( $"Dynamic selection worked. Wow. `{a.selected_option}`" );
                                                            slackApp.Update( this );
                                                        }
                                          }
                          },
                          new SectionLayout
                          {
                              block_id = "user-select-block",
                              text     = "User selection",
                              accessory = new UsersSelectElement
                                          {
                                              action_id = "what-user-selection",
                                              Clicked = ( s, _, a ) =>
                                                        {
                                                            _statusLayout.text =
                                                                new Markdown( $"User selection. `{a.selected_user}`" );
                                                            slackApp.Update( this );
                                                        }
                                          }
                          },
                          new SectionLayout
                          {
                              block_id = "conversation-select-block",
                              text     = "Conversation selection",
                              accessory = new ConversationSelectElement
                                          {
                                              action_id = "what-conversation-selection",
                                              Clicked = ( s, _, a ) =>
                                                        {
                                                            _statusLayout.text =
                                                                new Markdown( $"Conversation selection. `{a.selected_conversation}`" );
                                                            slackApp.Update( this );
                                                        }
                                          }
                          },
                          new SectionLayout
                          {
                              block_id = "channel-select-block",
                              text     = "Channel selection",
                              accessory = new ChannelSelectElement
                                          {
                                              action_id = "what-channel-selection",
                                              Clicked = ( s, _, a ) =>
                                                        {
                                                            _statusLayout.text =
                                                                new Markdown( $"Channel selection. `{a.selected_channel}`" );
                                                            slackApp.Update( this );
                                                        }
                                          }
                          },
                          new SectionLayout
                          {
                              block_id = "multi-select-block",
                              text     = new Markdown( "Fixed list for multiple selection" ),
                              accessory = new MultiSelectElement
                                          {
                                              action_id = "what-multi-selection",
                                              options = new List< Option >
                                                        {
                                                            new ( "One",   "select-one" ),
                                                            new ( "Two",   "select-two" ),
                                                            new ( "Three", "select-three" ),
                                                            new ( "Four",  "select-four" ),
                                                            new ( "Five",  "select-five" ),
                                                        },
                                              Clicked = ( s, _, a ) =>
                                                        {
                                                            _statusLayout.text =
                                                                new Markdown( $"Multi-selection is too much. `{string.Join( ',', a.selected_options )}`" );
                                                            slackApp.Update( this );
                                                        }
                                          }
                          },
                          new SectionLayout
                          {
                              block_id = "multi-select-external-block",
                              text     = "Dynamic list for multi selection",
                              accessory = new MultiSelectExternalElement
                                          {
                                              action_id   = "what-select-dynamic",
                                              placeholder = "The start of something dynamic",
                                              Suggestions = value =>
                                                                new Options
                                                                {
                                                                    options = new List< Option >
                                                                              {
                                                                                  new( value + " One", value   + "-select-one" ),
                                                                                  new( value + " Two", value   + "-select-two" ),
                                                                                  new( value + " Three", value + "-select-three" ),
                                                                                  new( value + " Four", value  + "-select-four" ),
                                                                                  new( value + " Five", value  + "-select-five" ),
                                                                              }
                                                                },
                                              Clicked = ( s, _, a ) =>
                                                        {
                                                            _statusLayout.text =
                                                                new Markdown( $"Dynamic multi-selection worked. Wow,wow,wow. `{string.Join( ',', a.selected_options )}`" );
                                                            slackApp.Update( this );
                                                        }
                                          }
                          },
                          new SectionLayout
                          {
                              block_id = "multi-user-select-block",
                              text     = new Markdown( "multiple user selection" ),
                              accessory = new MultiUserSelectElement
                                          {
                                              action_id = "what-multi-user-selection",
                                              Clicked = ( s, _, a ) =>
                                                        {
                                                            _statusLayout.text =
                                                                new Markdown( $"Multi-selection users. `{string.Join( ',', a.selected_users )}`" );
                                                            slackApp.Update( this );
                                                        }
                                          }
                          },
                          new SectionLayout
                          {
                              block_id = "multi-conversation-select-block",
                              text     = new Markdown( "multiple conversation selection" ),
                              accessory = new MultiConversationSelectElement
                                          {
                                              action_id = "what-multi-conversation-selection",
                                              Clicked = ( s, _, a ) =>
                                                        {
                                                            _statusLayout.text =
                                                                new Markdown( $"Multi-selection conversations. `{string.Join( ',', a.selected_conversations )}`" );
                                                            slackApp.Update( this );
                                                        }
                                          }
                          },
                          new SectionLayout
                          {
                              block_id = "multi-channel-select-block",
                              text     = new Markdown( "multiple channel selection" ),
                              accessory = new MultiChannelSelectElement
                                          {
                                              action_id = "what-multi-channel-selection",
                                              Clicked = ( s, _, a ) =>
                                                        {
                                                            _statusLayout.text =
                                                                new Markdown( $"Multi-selection channels. `{string.Join( ',', a.selected_channels )}`" );
                                                            slackApp.Update( this );
                                                        }
                                          }
                          },
                      };
            Add( layouts );
        }
    }
}

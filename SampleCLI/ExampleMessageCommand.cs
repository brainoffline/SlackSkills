using System;
using System.Collections.Generic;

using Asciis;

using SlackForDotNet;

namespace SampleCLI
{
    [ Command( "ExampleCommand", Description = "Example of using Slack as a command line interpreter" ) ]
    internal class ExampleMessageCommand : SlackMessageCommand
    {
        [ Param( "n|Name", "Name" ) ]
        public string? Name { get; set; }

        public override void OnCommand( List< string > extras )
        {
            if (extras.Count > 0)
                Name = extras[ 0 ];
            SlackApp?.SayToChannel( string.IsNullOrWhiteSpace( Name )
                                        ? "Example Hello"
                                        : $"Example Hello to {Name}. :wave:",
                                   null,
                                   channel: Message?.channel ?? "" );

            Console.WriteLine( "Example Command Executed" );
        }
    }
}

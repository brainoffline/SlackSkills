using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JetBrains.Annotations;

using SlackForDotNet;
using SlackForDotNet.Surface;

namespace BrainSlack
{
    public class EstimateChannelSurface : SlackSurface
    {
        public string Title       { get; set; }
        public string Description { get; set; }

        public EstimateChannelSurface( [ NotNull ] ISlackApp app ) : base( app )
        {
            Title = "Estimate story";
            Add( new HeaderLayout( "Join in the estimation" ) );
            Add( new SectionLayout() { text = $"Title: {Title}" } );
            if (!string.IsNullOrEmpty( Description ))
                Add(new SectionLayout() { text = $"{Description}" });
            Add( new DividerLayout() );
            Add( new ActionsLayout( "estimate-channel" )
                 {
                     elements = new List< Element >
                                {
                                    new ButtonElement("estimate-join", "Join")
                                    {
                                        Clicked = OnJoined
                                    }
                                }
                 } );
        }

        private void OnJoined( SlackSurface surface, ButtonElement button, BlockActions actions )
        {
            SlackApp!.Say( $"{actions.user.name} has joined", actions.channel.id );
        }
    }
}

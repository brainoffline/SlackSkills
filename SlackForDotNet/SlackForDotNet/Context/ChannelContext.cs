using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackForDotNet.Context
{
    public class ChannelContext
    {
        public string   ChannelId { get; }
        public Channel  Channel   { get; set; }

        public ChannelContext( string channelId, Channel channel )
        {
            this.ChannelId = channelId;
            Channel        = channel;
        }
    }
}

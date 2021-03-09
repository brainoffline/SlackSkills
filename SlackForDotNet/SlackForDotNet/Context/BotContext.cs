using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SlackForDotNet.WebApiContracts;

namespace SlackForDotNet.Context
{
    public class BotContext
    {
        public BotContext( string botId, Bot bot )
        {
            BotId = botId;
            Bot   = bot;
        }

        public string BotId { get; }
        public Bot    Bot   { get; set; }

    }
}

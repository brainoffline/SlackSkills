using System;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

namespace SlackForDotNet
{
    [ SlackMessage( "bot_added", apiType: Msg.RTM ) ]
    public class BotAdded : SlackMessage
    {
        public Bot    bot      { get; set; }
        //public int? cache_ts { get; set; }
    }

    [ SlackMessage( "bot_changed", apiType: Msg.RTM) ]
    public class BotChanged : SlackMessage
    {
        public Bot bot { get; set; }
    }
}

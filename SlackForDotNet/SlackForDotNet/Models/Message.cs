using System;
using System.Collections.Generic;

using SlackForDotNet.Surface;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

namespace SlackForDotNet
{
    [ SlackMessage( "message", apiType: Msg.RTM | Msg.Event ) ]
    public class Message : MessageBase
    {
        public string?         client_message_id     { get; set; }
        public string?         user_team             { get; set; }
        public string?         source_team           { get; set; }
        public string?         username              { get; set; }
        public string?         thread_ts             { get; set; }
        public Edited?         edited                { get; set; }
        public int?            reply_count           { get; set; }
        public bool?           is_starred            { get; set; }
        public bool?           suppress_notification { get; set; }
        public string?         permalink             { get; set; }
        public List<string>?   pinned_to             { get; set; }
        public List<Reply>?    replies               { get; set; }
        public int?            reply_users_count     { get; set; }
        public string?         latest_reply          { get; set; }
        public string?         event_ts              { get; set; }
        public List<Reaction>? reactions             { get; set; }
        public List<Layout>?   blocks                { get; set; }
        public Bot?            bot_profile           { get; set; }
    }
}

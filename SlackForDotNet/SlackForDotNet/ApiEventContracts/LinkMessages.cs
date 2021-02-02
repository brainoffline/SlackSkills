// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

using System;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

namespace SlackForDotNet
{
    [ SlackMessage( "link_shared", apiType: Msg.Event, scope: "links:read" ) ]
    public class LinkShared : SlackMessage
    {
        public string channel    { get; set; }
        public string user       { get; set; }
        public string message_ts { get; set; }
        public string ts         { get; set; }
        public Link[] links      { get; set; }

        public class Link
        {
            public string domain { get; set; }
            public string url    { get; set; }
        }
    }
}

using System;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

namespace SlackForDotNet.WebApiContracts
{
    [ SlackMessage( "views.open",
                      apiType: Msg.FormEncoded | Msg.Json | Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "bot" } ) ]
    public class ViewOpen : SlackMessage
    {
        public string? trigger_id { get; set; }
        public View?   view       { get; set; }
    }

    [ SlackMessage( "views.publish",
                      apiType: Msg.FormEncoded | Msg.Json | Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "bot" } ) ]
    public class ViewsPublish : SlackMessage
    {
        public string user_id { get; set; }
        public View view { get; set; }
        public string? hash { get; set; }
    }

    [ SlackMessage( "views.push",
                      apiType: Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "bot" } ) ]
    public class ViewPush : SlackMessage
    {
        
        public string user_id { get; set; }

        public string trigger_id { get; set; }

        public View view { get; set; }
    }

    [ SlackMessage( "views.update",
                      apiType: Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "bot", "channels:write" } ) ]
    public class ViewUpdate : SlackMessage
    {
        
        public View view { get; set; }

        public string? external_id { get; set; }

        public string? hash { get; set; }

        public string? view_id { get; set; }
    }

    public class ViewResponse : MessageResponse
    {
        public View view { get; set; }

        public MetaData? response_metadata { get; set; }

        public class MetaData
        {
            public string[] messages { get; set; }
        }
    }
}

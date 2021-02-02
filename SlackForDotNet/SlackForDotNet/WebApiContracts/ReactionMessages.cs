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
    [ SlackMessage( "reactions.add",
                      apiType: Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "reactions:write" } ) ]
    public class ReactionAdd : SlackMessage
    {
        
        public string channel { get; set; }

        
        public string name { get; set; }

        
        public string timestamp { get; set; }
    }

    /// <summary>
    /// https://api.slack.com/methods/reactions.get
    /// </summary>
    [ SlackMessage( "reactions.get",
                      apiType: Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "reactions:read" } ) ]
    public class ReactionsGet : SlackMessage
    {
        public string channel      { get; set; }
        public string file         { get; set; }
        public string file_comment { get; set; }
        public bool?  full         { get; set; }
        public string timestamp    { get; set; }
    }

    /// <summary>
    /// https://api.slack.com/methods/reactions.list
    /// </summary>
    [ SlackMessage( "reactions.list",
                      apiType: Msg.FormEncoded | Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "reactions:read" } ) ]
    public class ReactionsList : SlackMessage
    {
        public int?   count  { get; set; }
        public string cursor { get; set; }
        public bool?  full   { get; set; }
        public int?   limit  { get; set; }
        public int?   page   { get; set; }
        public string user   { get; set; }
    }

    [ SlackMessage( "reactions.remove",
                      apiType: Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "reactions:write" } ) ]
    public class ReactionRemove : SlackMessage
    {
        
        public string channel { get; set; }

        
        public string name { get; set; }

        
        public string timestamp { get; set; }
    }
}

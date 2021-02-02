// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

using System;
using System.Collections.Generic;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

namespace SlackForDotNet.WebApiContracts
{
    [ SlackMessage( "admin.emoji.add",
                      apiType: Msg.FormEncoded | Msg.EnterpriseGrid,
                      scopes: new[] { "admin.teams:write" } ) ]
    public class AdminEmojiAdd : SlackMessage
    {
        
        public string name { get; set; }

        
        public string url { get; set; }
    }

    [ SlackMessage( "admin.emoji.addAlias",
                      apiType: Msg.FormEncoded | Msg.EnterpriseGrid,
                      scopes: new[] { "admin.teams:write" } ) ]
    public class AdminEmojiAddAlias : SlackMessage
    {
        
        public string alias_for { get; set; }

        
        public string name { get; set; }
    }

    [ SlackMessage( "admin.emoji.list",
                      apiType: Msg.FormEncoded | Msg.EnterpriseGrid,
                      scopes: new[] { "admin.teams:read" } ) ]
    public class AdminEmojiList : SlackMessage
    {
        public string cursor { get; set; }
        public int?   limit  { get; set; }
    }

    /// <summary>
    ///     This class may have to be manually decode as Slack does not have a consistant schema
    ///     The emoji value can be either a string or a dictionary
    /// <code>
    /// {
    ///   "emoji": {
    ///     "bowtie": "https://emoji.slack-edge.com/T9TK3CUKW/bowtie/f3ec6f2bb0.png",
    ///     "simple_smile": {
    ///       "apple": "https://a.slack-edge.com/80588/img/emoji_2017_12_06/apple/simple_smile.png",
    ///       "google": "https://a.slack-edge.com/80588/img/emoji_2017_12_06/google/simple_smile.png"
    ///     }
    /// },
    /// </code>
    /// </summary>
    public class AdminEmojiListResponse : MessageResponse
    {
        public int?                       cache_ts           { get; set; }
        public Dictionary< string, string > emoji              { get; set; }
        public string                       categories_version { get; set; }
        public EmojiCategory[]              categories         { get; set; }
    }

    [ SlackMessage( "admin.emoji.remove",
                      apiType: Msg.FormEncoded | Msg.EnterpriseGrid,
                      scopes: new[] { "admin.teams:write" } ) ]
    public class AdminEmojiRemove : SlackMessage
    {
        
        public string name { get; set; }
    }

    [ SlackMessage( "admin.emoji.rename",
                      apiType: Msg.FormEncoded | Msg.EnterpriseGrid,
                      scopes: new[] { "admin.teams:write" } ) ]
    public class AdminEmojiRename : SlackMessage
    {
        
        public string name { get; set; }

        
        public string new_name { get; set; }
    }
}

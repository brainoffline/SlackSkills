// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

using System;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

namespace SlackSkills.WebApiContracts
{
    [ SlackMessage( "stars.add",
                      apiType: Msg.FormEncoded | Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "stars:write" } ) ]
    public class StarAdd : SlackMessage
    {
        public string channel      { get; set; }
        public string file         { get; set; }
        public string file_comment { get; set; }
        public string timestamp    { get; set; }
    }

    [ SlackMessage( "stars.list",
                      apiType: Msg.FormEncoded | Msg.UserToken,
                      scopes: new[] { "stars:read" } ) ]
    public class StarsList : SlackMessage
    {
        public int?   count  { get; set; }
        public string cursor { get; set; }
        public int?   limit  { get; set; }
        public int?   page   { get; set; }
    }

    [ SlackMessage( "stars.remove",
                      apiType: Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "bot", "stars:write" } ) ]
    public class StarRemove : SlackMessage
    {
        public string channel      { get; set; }
        public string file         { get; set; }
        public string file_comment { get; set; }
        public string timestamp    { get; set; }
    }
}

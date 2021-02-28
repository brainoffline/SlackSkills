// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.VisualBasic.CompilerServices;

using Newtonsoft.Json;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

namespace SlackForDotNet.WebApiContracts
{
    [ SlackMessage( "admin.conversations.archive",
                      apiType: Msg.FormEncoded | Msg.Json | Msg.EnterpriseGrid,
                      scopes: new[] { "admin.conversations:write" } ) ]
    public class AdminConversationsArchive : SlackMessage
    {
        public string channel_id { get; set; }
    }

    [ SlackMessage( "admin.conversations.convertToPrivate",
                      apiType: Msg.FormEncoded | Msg.Json | Msg.EnterpriseGrid,
                      scopes: new[] { "admin.conversations:write" } ) ]
    public class AdminConversationsConvertToPrivate : SlackMessage
    {
        public string channel_id { get; set; }
    }

    [ SlackMessage( "admin.conversations.create",
                      apiType: Msg.FormEncoded | Msg.Json | Msg.EnterpriseGrid,
                      scopes: new[] { "admin.conversations:write" } ) ]
    public class AdminConversationsCreate : SlackMessage
    {
        public string  channel_id  { get; set; }
        public bool    is_private  { get; set; }
        public string  name        { get; set; }
        public string? description { get; set; }
        public bool?   org_wide    { get; set; }
        public string? team_id     { get; set; }
    }

    [ SlackMessage( "admin.conversations.delete",
                      apiType: Msg.FormEncoded | Msg.Json | Msg.EnterpriseGrid,
                      scopes: new[] { "admin.conversations:write" } ) ]
    public class AdminConversationsDelete : SlackMessage
    {
        public string channel_id { get; set; }
    }

    [ SlackMessage( "admin.conversations.getConversationPrefs",
                      apiType: Msg.FormEncoded | Msg.Json | Msg.EnterpriseGrid,
                      scopes: new[] { "admin.conversations:read" } ) ]
    public class AdminConversationsGetConversationPrefs : SlackMessage
    {
        public string channel_id { get; set; }
    }

    [ SlackMessage( "admin.conversations.getCustomRetention",
                      apiType: Msg.FormEncoded | Msg.Json | Msg.EnterpriseGrid,
                      scopes: new[] { "admin.conversations:read" } ) ]
    public class AdminConversationsGetCustomRetention : SlackMessage
    {
        public string channel_id { get; set; }
    }

    [ SlackMessage( "admin.conversations.getTeams",
                      apiType: Msg.FormEncoded | Msg.Json | Msg.EnterpriseGrid,
                      scopes: new[] { "admin.conversations:read" } ) ]
    public class AdminConversationsGetTeams : SlackMessage
    {
        public string  channel_id { get; set; }
        public string? cursor     { get; set; }
        public int?    limit      { get; set; }
    }

    public class AdminConversationsGetTeamsResponse : MessageResponse
    {
        public string Teams { get; set; }
    }

    [ SlackMessage( "admin.conversations.invite",
                      apiType: Msg.FormEncoded | Msg.Json | Msg.EnterpriseGrid,
                      scopes: new[] { "admin.conversations:write" } ) ]
    public class AdminConversationsInvite : SlackMessage
    {
        public string  channel_id { get; set; }
        public string? user_ids   { get; set; }
    }

    [ SlackMessage( "admin.conversations.removeCustomRetention",
                      apiType: Msg.FormEncoded | Msg.Json | Msg.EnterpriseGrid,
                      scopes: new[] { "admin.conversations:write" } ) ]
    public class AdminConversationsRemoveCustomRetention : SlackMessage
    {
        public string channel_id { get; set; }
    }

    [ SlackMessage( "admin.conversations.rename",
                      apiType: Msg.FormEncoded | Msg.Json | Msg.EnterpriseGrid,
                      scopes: new[] { "admin.conversations:write" } ) ]
    public class AdminConversationsRename : SlackMessage
    {
        public string channel_id { get; set; }
        public string name       { get; set; }
    }

    [ SlackMessage( "admin.conversations.search",
                      apiType: Msg.FormEncoded | Msg.Json | Msg.EnterpriseGrid,
                      scopes: new[] { "admin.conversations:read" } ) ]
    public class AdminConversationsSearch : SlackMessage
    {
        public string? cursor               { get; set; }
        public int?    limit                { get; set; }
        public string? query                { get; set; }
        public string? search_channel_types { get; set; }
        public string? sort                 { get; set; }
        public string? sort_dir             { get; set; }
        public string? team_ids             { get; set; }
    }

    public class AdminConversationsSearchResponse : MessageResponse
    {
        public List< Message > conversations { get; set; }
        public string?         next_cursor   { get; set; }
    }

    [ SlackMessage( "admin.conversations.setConversationPrefs",
                      apiType: Msg.FormEncoded | Msg.Json | Msg.EnterpriseGrid,
                      scopes: new[] { "admin.conversations:write" } ) ]
    public class AdminConversationsSetConversationPrefs : SlackMessage
    {
        public string channel_id { get; set; }
        public object prefs      { get; set; }
    }

    [ SlackMessage( "admin.conversations.setCustomRetention",
                      apiType: Msg.FormEncoded | Msg.Json | Msg.EnterpriseGrid,
                      scopes: new[] { "admin.conversations:write" } ) ]
    public class AdminConversationsSetCustomRetention : SlackMessage
    {
        public string channel_id    { get; set; }
        public int    duration_days { get; set; }
    }

    [ SlackMessage( "admin.conversations.setTeams",
                      apiType: Msg.FormEncoded | Msg.Json | Msg.EnterpriseGrid,
                      scopes: new[] { "admin.conversations:write" } ) ]
    public class AdminConversationsSetTeams : SlackMessage
    {
        public string  channel_id      { get; set; }
        public bool?   org_channel     { get; set; }
        public string? target_team_ids { get; set; }
        public string? team_id         { get; set; }
    }

    [SlackMessage( "admin.conversations.unarchive",
                     apiType: Msg.FormEncoded | Msg.Json | Msg.EnterpriseGrid,
                     scopes: new[] { "admin.conversations:write" })]
    public class AdminConversationsUnarchive : SlackMessage
    {
        public string channel_id { get; set; }
    }
}
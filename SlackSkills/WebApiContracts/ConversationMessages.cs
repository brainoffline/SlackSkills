using System;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

namespace SlackSkills.WebApiContracts
{
    [ SlackMessage( "conversations.archive",
                      apiType: Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "groups:write", "im:write", "mpim:write", "channels:write" } ) ]
    public class ConversationArchive : SlackMessage
    {
        
        public string channel { get; set; }
    }

    [ SlackMessage( "conversations.close",
                      apiType: Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "groups:write", "im:write", "mpim:write", "channels:write" } ) ]
    public class ConversationClose : SlackMessage
    {
        
        public string channel { get; set; }
    }

    /// <summary>
    /// Response is ChannelResponse
    /// </summary>
    [ SlackMessage( "conversations.create",
                      apiType: Msg.UserToken,
                      scopes: new[] { "channels:write", "groups:write", "im:write", "mpim:write" } ) ]
    public class ConversationCreate : SlackMessage
    {
        
        public string name { get; set; }

        
        public bool? is_private { get; set; }

        
        public string[]? user_ids { get; set; }
    }

    [ SlackMessage( "conversations.history",
                      apiType: Msg.FormEncoded | Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "channels:history", "groups:history", "im:history", "mpim:history" } ) ]
    public class ConversationHistory : SlackMessage
    {
        
        public string channel { get; set; }

        public string cursor    { get; set; }
        public int?   limit     { get; set; }
        public int?   inclusive { get; set; }
        public string latest    { get; set; }
        public string oldest    { get; set; }
    }

    [ SlackMessage( "conversations.info",
                      apiType: Msg.FormEncoded | Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "channels:read", "groups:read", "im:read", "mpim:read" } ) ]
    public class ConverationInfo : SlackMessage
    {
        
        public string channel { get; set; }

        public bool? include_locale { get; set; }
        public bool? include_num_members { get; set; }
    }

    public class ConversationInfoResponse : MessageResponse
    {
        public Channel channel { get; set; }
    }

    /// <summary>
    /// Response is ChannelResponse
    /// </summary>
    [ SlackMessage( "conversations.invite",
                      apiType: Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "channels:write", "groups:write", "im:write", "mpim:write" } ) ]
    public class ConversationInvite : SlackMessage
    {
        
        public string channel { get; set; }

        
        public string[]? users { get; set; }
    }

    [ SlackMessage( "conversations.join",
                      apiType: Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "bot", "channels:write" } ) ]
    public class ConversationJoin : SlackMessage
    {
        
        public string channel { get; set; }
    }

    public class ConversationJoinResponse : MessageResponse
    {
        public Channel  channel           { get; set; }
        public MetaData response_metadata { get; set; }

        public class MetaData
        {
            public string[] warnings { get; set; }
        }
    }

    [ SlackMessage( "conversations.kick",
                      apiType: Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "channels:write", "groups:write", "im:write", "mpim:write" }
                  ) ]
    public class ConversationKick : SlackMessage
    {
        
        public string channel { get; set; }

        
        public string user { get; set; }
    }

    [ SlackMessage( "conversations.leave",
                      apiType: Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "channels:write", "groups:write", "im:write", "mpim:write" }
                  ) ]
    public class ConversationLeave : SlackMessage
    {
        
        public string channel { get; set; }
    }

    /// <summary>
    ///     Response is ConversionsResponse
    /// </summary>
    [ SlackMessage( "conversations.list",
                      apiType: Msg.FormEncoded | Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "channels:read", "groups:read", "im:read", "mpim:read" } ) ]
    public class ConversationList : SlackMessage
    {
        public string cursor           { get; set; }
        public int?   limit            { get; set; }
        public bool?  exclude_archived { get; set; }

        /// <summary>
        /// Possible values: public_channel, private_channel, mpim, im
        /// </summary>
        public string types { get; set; }
    }

    [ SlackMessage( "conversations.members",
                      apiType: Msg.FormEncoded | Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "channels:read", "groups:read", "im:read", "mpim:read" } ) ]
    public class ConversationMembers : SlackMessage
    {
        
        public string channel { get; set; }

        public string cursor { get; set; }
        public int?   limit  { get; set; }
    }

    public class ConversationMembersResponse : MessageResponse
    {
        public string[] members           { get; set; }
        public MetaData response_metadata { get; set; }
    }

    [ SlackMessage( "conversations.open",
                      apiType: Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "channels:write", "groups:write", "im:write", "mpim:write" } ) ]
    public class ConversationOpen : SlackMessage
    {
        
        public string? channel { get; set; }

        
        public bool? return_im { get; set; }

        
        public string[]? users { get; set; }
    }

    public class ConversationOpenResponse : MessageResponse
    {
        public bool?   already_open { get; set; }
        public Channel channel      { get; set; }
    }

    [ SlackMessage( "conversations.rename",
                      apiType: Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "channels:write", "groups:write", "im:write", "mpim:write" } ) ]
    public class ConversationRename : SlackMessage
    {
        
        public string channel { get; set; }

        
        public string name { get; set; }
    }


    /// <summary>
    ///     Response is ConversionResponse
    /// </summary>
    [ SlackMessage( "conversations.replies",
                      apiType: Msg.FormEncoded | Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "channels:history", "groups:history", "im:history", "mpim:history" } ) ]
    public class ConversationReplies : SlackMessage
    {
        
        public string channel { get; set; }

        
        public string ts { get; set; }

        public string cursor    { get; set; }
        public bool?  inclusive { get; set; }
        public int?   limit     { get; set; }
        public string oldest    { get; set; }
        public string latest    { get; set; }
    }

    [ SlackMessage( "conversations.setPurpose",
                      apiType: Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "channels:write", "channels.manage", "groups:write", "im:write", "mpim:write" } ) ]
    public class ConversationSetPurpose : SlackMessage
    {
        
        public string channel { get; set; }

        
        public string purpose { get; set; }
    }

    public class ConversationSetPurposeResponse : MessageResponse
    {
        public string purpose { get; set; }
    }

    [ SlackMessage( "conversations.setTopic",
                      apiType: Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "channels:write", "channels.manage", "groups:write", "im:write", "mpim:write" } ) ]
    public class ConversationSetTopic : SlackMessage
    {
        
        public string channel { get; set; }

        
        public string topic { get; set; }
    }

    public class ConversationSetTopicResponse : MessageResponse
    {
        public string topic { get; set; }
    }

    [ SlackMessage( "conversations.unarchive",
                      apiType: Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "channels:write", "channels.manage", "groups:write", "im:write", "mpim:write" } ) ]
    public class ConversationUnarchive : SlackMessage
    {
        
        public string channel { get; set; }
    }

    public class ConversationsResponse : MessageResponse
    {
        public Channel[] channels { get; set; }
        public MetaData metadata { get; set; }
    }

    public class ConversationResponse : MessageResponse
    {
        public MessageBase[] messages          { get; set; }
        public Channel       channel           { get; set; }
        public bool?         has_more          { get; set; }
        public int?          pin_count         { get; set; }
        public MetaData      response_metadata { get; set; }
    }
}

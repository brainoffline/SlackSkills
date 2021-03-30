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
    [ SlackMessage( "users.conversations",
                      apiType: Msg.FormEncoded | Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "channels:read", "groups:read", "im:read", "mpim:read" } ) ]
    public class UserConversations : SlackMessage
    {
        public string cursor { get; set; }

        public bool? exclude_archived { get; set; }

        public int? limit { get; set; }

        /// <summary>
        /// Possible values: public_channel, private_channel, mpim, im
        /// </summary>
        public string types { get; set; }

        public string user { get; set; }
    }

    public class UserConversationsResponse : MessageResponse
    {
        public Channel[] channels          { get; set; }
        public MetaData  response_metadata { get; set; }
    }

    [ SlackMessage( "users.deletePhoto",
                      apiType: Msg.FormEncoded | Msg.UserToken,
                      scopes: new[] { "user.profile:write" } ) ]
    public class UserDeletePhoto : SlackMessage { }

    [ SlackMessage( "users.getpresence",
                      apiType: Msg.FormEncoded | Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "users:read" } ) ]
    public class UserGetPresence : SlackMessage { }

    [ SlackMessage( "users.identity",
                      apiType: Msg.FormEncoded | Msg.UserToken,
                      scopes: new[] { "identity:basic" } ) ]
    public class UserIdentity : SlackMessage { }

    public class UserIdentityResponse : MessageResponse
    {
        public User user { get; set; }
        public Team team { get; set; }
    }

    [ SlackMessage( "users.info",
                      apiType: Msg.FormEncoded | Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "users:read" } ) ]
    public class UserInfo : SlackMessage
    {
        public string user { get; set; }

        public bool? include_locale { get; set; }
    }

    public class UserInfoResponse : MessageResponse
    {
        public User user { get; set; }
    }

    [ SlackMessage( "users.list",
                      apiType: Msg.FormEncoded | Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "users:read" } ) ]
    public class UserList : SlackMessage
    {
        public string cursor { get; set; }

        public bool? include_locale { get; set; }

        public int? limit { get; set; }
    }

    public class UserListResponse : MessageResponse
    {
        public int? cache_ts { get; set; }

        public User[] members { get; set; }

        public MetaData response_metadata { get; set; }
    }

    [ SlackMessage( "users.lookupByEmail",
                      apiType: Msg.FormEncoded | Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "users:read.email" } ) ]
    public class UserLookupByEmail : SlackMessage
    {
        public string email { get; set; }
    }

    public class UserLookupResponse : MessageResponse
    {
        public User user { get; set; }
    }

    /// <summary>
    /// https://api.slack.com/methods/users.setPhoto
    /// </summary>
    [ SlackMessage( "users.setPhoto",
                      apiType: Msg.Multipart | Msg.UserToken,
                      scopes: new[] { "users.profile:write" } ) ]
    public class UserSetPhoto : SlackMessage
    {
        public int? crop_w { get; set; }
        public int? crop_x { get; set; }
        public int? crop_y { get; set; }
    }

    [ SlackMessage( "users.setPresence",
                      apiType: Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "users:write" } ) ]
    public class UserSetPresence : SlackMessage
    {
        
        public string presence { get; set; }
    }

    [ SlackMessage( "users.profile.get",
                      apiType: Msg.FormEncoded | Msg.UserToken,
                      scopes: new[] { "users.profile:read" } ) ]
    public class UserProfileGet : SlackMessage
    {
        public bool? include_labels { get; set; }

        public string user { get; set; }
    }

    public class UserProfileGetResponse : MessageResponse
    {
        public UserProfile profile { get; set; }
    }

    [ SlackMessage( "users.profile.set",
                      apiType: Msg.UserToken,
                      scopes: new[] { "users.profile:write" } ) ]
    public class UserProfileSet : SlackMessage
    {
        
        public string? name { get; set; }

        
        public UserProfile? profile { get; set; }

        
        public string? user { get; set; }

        
        public string? value { get; set; }
    }
}

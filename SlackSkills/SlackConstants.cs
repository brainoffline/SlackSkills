using System;

namespace SlackSkills
{
    public static class SlackConstants
    {
        // ReSharper disable InconsistentNaming
        // ReSharper disable StringLiteralTypo

        public static string BuildScope( params string[] scopes ) =>
            string.Join( ",", scopes );

        public static class BotScopes
        {
            public const string AppMentions_Read       = "app_mentions:read";
            public const string Calls_Read             = "calls:read";
            public const string Calls_Write            = "calls:write";
            public const string Channels_History       = "channels:history";
            public const string Channels_Join          = "channels:join";
            public const string Channels_Manage        = "channels:manage";
            public const string Channels_Read          = "channels:read";
            public const string Chat_Write             = "chat:write";
            public const string Chat_Write_Customize   = "chat:write.customize";
            public const string Chat_Write_Public      = "chat:write.public";
            public const string Commands               = "commands";
            public const string DoNotDisturb_Read      = "dnd:read";
            public const string Emoji_Read             = "emoji:read";
            public const string Files_Read             = "files:read";
            public const string Files_Write            = "files:write";
            public const string Groups_History         = "groups:history";
            public const string Groups_Read            = "groups:read";
            public const string Groups_Write           = "groups:write";
            public const string InstantMessage_History = "im:history";
            public const string InstantMessage_Read    = "im:read";
            public const string InstantMessage_Write   = "im:write";
            public const string IncomingWebhook        = "incoming-webhook";
            public const string Links_Read             = "links:read";
            public const string Links_Write            = "links:write";
            public const string MPIM_History           = "mpim:history";
            public const string MPIM_Read              = "mpim:read";
            public const string MPIM_Write             = "mpim:write";
            public const string None                   = "none";
            public const string Pins_Read              = "pins:read";
            public const string Pins_Write             = "pins:write";
            public const string Reactions_Read         = "reactions:read";
            public const string Reactions_Write        = "reactions:write";
            public const string Reminders_Read         = "reminders:read";
            public const string Reminders_Write        = "reminders:write";
            public const string RemoteFiles_Read       = "remote_files:read";
            public const string RemoteFiles_Share      = "remote_files:share";
            public const string RemoteFiles_Write      = "remote_files:write";
            public const string Team_Read              = "team:read";
            public const string UserGroups_Read        = "usergroups:read";
            public const string UserGroups_Write       = "usergroups:write";
            public const string UsersProfile_Read      = "users.profile:read";
            public const string Users_Read             = "users:read";
            public const string Users_Read_Email       = "users:read.email";
            public const string Users_Write            = "users:write";
            public const string WorkflowSteps_Execute  = "workflow.steps:execute";
        }

        public static class UserScopes
        {
            public const string Admin                    = "admin";
            public const string AdminAnalytics_Read      = "admin.analytics:read";
            public const string AdminApps_Read           = "admin.apps:read";
            public const string AdminApps_Write          = "admin.apps:write";
            public const string AdminBarriers_Read       = "admin.barriers:read";
            public const string AdminBarriers_Write      = "admin.barriers:write";
            public const string AdminConversations_Read  = "admin.conversations:read";
            public const string AdminConversations_Write = "admin.conversations:write";
            public const string AdminInvites_Read        = "admin.invites:read";
            public const string AdminInvites_Write       = "admin.invites:write";
            public const string AdminTeams_Read          = "admin.teams:read";
            public const string AdminTeams_Write         = "admin.teams:write";
            public const string AdminUsergroups_Read     = "admin.usergroups:read";
            public const string AdminUsergroups_Write    = "admin.usergroups:write";
            public const string AdminUsers_Read          = "admin.users:read";
            public const string AdminUsers_Write         = "admin.users:write";
            public const string AuditLogs_Read           = "auditlogs:read";
            public const string Calls_Read               = "calls:read";
            public const string Calls_Write              = "calls:write";
            public const string Channels_History         = "channels:history";
            public const string Channels_Read            = "channels:read";
            public const string Channels_Write           = "channels:write";
            public const string Chat_Write               = "chat:write";
            public const string Chat_Write_User          = "chat:write:user";
            public const string Chat_Write_Bot           = "chat:write:bot";
            public const string Commands                 = "commands";
            public const string DoNotDisturb_Read        = "dnd:read";
            public const string DoNotDisturb_Write       = "dnd:write";
            public const string Emoji_Read               = "emoji:read";
            public const string Files_Read               = "files:read";
            public const string Files_Write              = "files:write";
            public const string Files_Write_User         = "files:write:user";
            public const string Groups_History           = "groups:history";
            public const string Groups_Read              = "groups:read";
            public const string Groups_Write             = "groups:write";
            public const string Identity_Avatar          = "identity.avatar";
            public const string Identity_Basic           = "identity.basic";
            public const string Identity_Email           = "identity.email";
            public const string Identity_Team            = "identity.team";
            public const string InstantMessage_History   = "im:history";
            public const string InstantMessage_Read      = "im:read";
            public const string InstantMessage_Write     = "im:write";
            public const string IncomingWebhook          = "incoming-webhook";
            public const string Links_Read               = "links:read";
            public const string Links_Write              = "links:write";
            public const string MPIM_History             = "mpim:history";
            public const string MPIM_Read                = "mpim:read";
            public const string MPIM_Write               = "mpim:write";
            public const string Pins_Read                = "pins:read";
            public const string Pins_Write               = "pins:write";
            public const string Reactions_Read           = "reactions:read";
            public const string Reactions_Write          = "reactions:write";
            public const string Reminders_Read           = "reminders:read";
            public const string Reminders_Write          = "reminders:write";
            public const string RemoteFiles_Read         = "remote_files:read";
            public const string RemoteFiles_Share        = "remote_files:share";
            public const string Search_Read              = "search:read";
            public const string Stars_Read               = "stars:read";
            public const string Stars_Write              = "stars:write";
            public const string Team_Read                = "team:read";
            public const string Tokens_Basic             = "tokens.basic";
            public const string UserGroups_Read          = "usergroups:read";
            public const string UserGroups_Write         = "usergroups:write";
            public const string UsersProfile_Read        = "users.profile:read";
            public const string UsersProfile_Write       = "users.profile:write";
            public const string Users_Read               = "users:read";
            public const string Users_Read_Email         = "users:read.email";
            public const string Users_Write              = "users:write";
        }

        // ReSharper restore StringLiteralTypo
        // ReSharper restore InconsistentNaming

        public const string DefaultRedirectUrl = "http://localhost:3100/";
    }
}

using System;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

namespace SlackSkills
{
    public class User
    {
        public string  id        { get; set; }
        public string? name      { get; set; }
        public string? team_id   { get; set; }
        public string? real_name { get; set; }
        public string? username  { get; set; }
        public string? color     { get; set; }

        public UserProfile? profile { get; set; }

        public bool? always_active       { get; set; }
        public bool? deleted             { get; set; }
        public bool? is_admin            { get; set; }
        public bool? is_owner            { get; set; }
        public bool? is_primary_owner    { get; set; }
        public bool? is_restricted       { get; set; }
        public bool? is_ultra_restricted { get; set; }
        public bool? is_app_user         { get; set; }
        public bool? is_bot              { get; set; }
        public bool? is_invited_user     { get; set; }
        public bool? is_stranger         { get; set; }
        public bool? has_2fa             { get; set; }
        public bool? has_files           { get; set; }

        public string?     two_factor_type { get; set; }
        public string?     presence        { get; set; }
        public string?     tz              { get; set; }
        public string?     tz_label        { get; set; }
        public int?        tz_offset       { get; set; }
        public int?        updated         { get; set; }
        public string?     locale          { get; set; }
        public Enterprise? enterprise_user { get; set; }

        public bool IsSlackBot => id.Equals( "USLACKBOT", StringComparison.CurrentCultureIgnoreCase );
    }
    
    public class AuthedUser
    {
        public string id           { get; set; }
        public string scope        { get; set; }
        public string access_token { get; set; }
        public string token_type   { get; set; }
    }
    
    public class UserMinimal
    {
        public string  id       { get; set; }
        public string? username { get; set; }
        public string? name     { get; set; }
        public string? team_id  { get; set; }
    }
}

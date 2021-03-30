// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized

using System;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

namespace SlackSkills
{
    public class UserGroup
    {
        public string  id          { get; set; }
        public string? team_id     { get; set; }
        public string? name        { get; set; }
        public string? description { get; set; }
        public string? handle      { get; set; }

        public bool? is_usergroup { get; set; }
        public bool? is_external  { get; set; }

        public int?     date_create { get; set; }
        public int?     date_update { get; set; }
        public int?     date_delete { get; set; }
        public string? auto_type   { get; set; }
        public string? created_by  { get; set; }
        public string? updated_by  { get; set; }
        public string? deleted_by  { get; set; }

        public Preferences? prefs { get; set; }

        public string[]? users      { get; set; }
        public string?   user_count { get; set; }

        public class Preferences
        {
            public string[]? channels { get; set; }
            public string[]? groups   { get; set; }
        }
    }
}

using System;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null


// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

namespace SlackForDotNet
{
    public class SubTeam
    {
        public string  id           { get; set; }
        public string  team_id      { get; set; }
        public bool?   is_usergroup { get; set; }
        public string  name         { get; set; }
        public string? description  { get; set; }
        public string? handle       { get; set; }

        public bool? is_external { get; set; }
        public int?  date_create { get; set; }
        public int?  date_update { get; set; }
        public int?  date_delete { get; set; }

        
        public string? auto_type { get; set; }

        
        public string? created_by { get; set; }

        
        public string? updated_by { get; set; }

        
        public string? deleted_by { get; set; }

        
        public SubTeamPrefs? prefs { get; set; }

        public string[] users      { get; set; }
        public int      user_count { get; set; }
    }
}

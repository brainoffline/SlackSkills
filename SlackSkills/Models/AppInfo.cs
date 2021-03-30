// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

using System;

#pragma warning disable 8618

namespace SlackSkills
{
    public class AppInfo
    {
        public string id                        { get; set; }
        public string name                      { get; set; }
        public string description               { get; set; }
        public string help_url                  { get; set; }
        public string privacy_policy_url        { get; set; }
        public string app_homepage_url          { get; set; }
        public string app_directory_url         { get; set; }
        public bool?  is_app_directory_approved { get; set; }
        public bool?  is_internal               { get; set; }
        public string additional_info           { get; set; }
    }

    public class AppDetailedInfo
    {
        public AppInfo app { get; set; }

        public Scopes[] scopes           { get; set; }
        public int?     date_updated     { get; set; }
        public Actor    last_resolved_by { get; set; }

        public class Actor
        {
            public string actor_id   { get; set; }
            public string actor_type { get; set; }
        }
    }

    public class AppRequestedInfo
    {
        public AppInfo app { get; set; }

        public string   previous_resolution { get; set; }
        public User     user                { get; set; }
        public Team     team                { get; set; }
        public Scopes[] scopes              { get; set; }
        public string   message             { get; set; }
        public int      date_created        { get; set; }
    }
}

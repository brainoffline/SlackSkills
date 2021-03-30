// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

using System.Collections.Generic;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

namespace SlackSkills.WebApiContracts
{
    /// <summary>
    /// Retrieve analytics. Currently only member supported
    /// </summary>
    /// <returns>Non traditional json.  One json record per line. Response will be gzipped</returns>
    [SlackMessage("admin.analytics.getFile",
                     apiType: Msg.FormEncoded | Msg.UserToken,
                     scopes: new[] { "admin.analytics:read" })]
    public class AdminAnalyticsGetFile : SlackMessage
    {
        public string? date          { get; set; } // yyyy-mm-dd e.g. 2020-09-01
        public bool?   metadata_only { get; set; }
    }

    public class AdminAnalyticsGetFileResponse : List< string > { }

    public class AnalyticsMemberLine
    {
        public string    date                          { get; set; }
        public string    enterprise_id                 { get; set; }
        public string    enterprise_user_id            { get; set; }
        public string    email_address                 { get; set; }
        public string    enterprise_employee_number    { get; set; }
        public bool      is_guest                      { get; set; }
        public bool      is_billable_seat              { get; set; }
        public bool      is_active                     { get; set; }
        public bool      is_Active_ios                 { get; set; }
        public bool      is_active_android             { get; set; }
        public bool      is_active_desktop             { get; set; }
        public int       reactions_added_count         { get; set; }
        public int       messages_posted_count         { get; set; }
        public int       channel_messages_posted_count { get; set; }
        public int       files_added_count             { get; set; }
    }

    public class AnalyticsConversationLine
    {
        public string       enterprise_id                    { get; set; }
        public TeamName     originating_team                 { get; set; }
        public string       channel_id                       { get; set; }
        public string       channel_type                     { get; set; }
        public string       visibility                       { get; set; }
        public List< Team > shared_with                      { get; set; }
        public bool?        is_shared_externally             { get; set; }
        public int          date_created                     { get; set; }
        public int          date_last_active                 { get; set; }
        public int          total_members_count              { get; set; }
        public int          full_members_count               { get; set; }
        public int          active_members_count             { get; set; }
        public int          guest_member_count               { get; set; }
        public int          messages_posted_count            { get; set; }
        public int          messages_posted_by_members_count { get; set; }
        public int          members_who_posted_count         { get; set; }
        public int          members_who_viewed_count         { get; set; }
        public int          reactions_added_count            { get; set; }
        public string       date                             { get; set; }
    }

    public class AnalyticsPublicChannelLine
    {
        public string channel_id  { get; set; }
        public string name        { get; set; }
        public string topic       { get; set; }
        public string description { get; set; }
        public string date        { get; set; }
    }
}


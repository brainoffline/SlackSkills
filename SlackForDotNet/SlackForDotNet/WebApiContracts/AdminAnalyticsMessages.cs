// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

namespace SlackForDotNet.WebApiContracts
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
        public string date { get; set; }   // yyyy-mm-dd e.g. 2020-09-01
    }


    public class AnalyticsMemberLine
    {
        public string date                          { get; set; }
        public string enterprise_id                 { get; set; }
        public string enterprise_user_id            { get; set; }
        public string email_address                 { get; set; }
        public string enterprise_employee_number    { get; set; }
        public bool   is_guest                      { get; set; }
        public bool   is_billable_seat              { get; set; }
        public bool   is_active                     { get; set; }
        public bool   is_Active_ios                 { get; set; }
        public bool   is_active_android             { get; set; }
        public bool   is_active_desktop             { get; set; }
        public int    reactions_added_count         { get; set; }
        public int    messages_posted_count         { get; set; }
        public int    channel_messages_posted_count { get; set; }
        public int    files_added_count             { get; set; }
    }
}


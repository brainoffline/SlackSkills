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
    [ SlackMessage( "reminders.add",
                      apiType: Msg.UserToken,
                      scopes: new[] { "reminders:write" } ) ]
    public class ReminderAdd : SlackMessage
    {
        
        public string text { get; set; }

        public int time { get; set; }

        
        public string? user { get; set; }
    }

    [ SlackMessage( "reminders.complete",
                      apiType: Msg.UserToken,
                      scopes: new[] { "reminders:write" } ) ]
    public class ReminderComplete : SlackMessage
    {
        
        public string reminder { get; set; }
    }

    [ SlackMessage( "reminders.delete",
                      apiType: Msg.UserToken,
                      scopes: new[] { "reminders:write" } ) ]
    public class ReminderDelete : SlackMessage
    {
        
        public string reminder { get; set; }
    }

    [ SlackMessage( "reminders.info",
                      apiType: Msg.FormEncoded | Msg.UserToken,
                      scopes: new[] { "reminders:read" } ) ]
    public class RemindersInfo : SlackMessage
    {
        public string reminder { get; set; }
    }

    [ SlackMessage( "reminders.list",
                      apiType: Msg.FormEncoded | Msg.UserToken,
                      scopes: new[] { "reminders:read" } ) ]
    public class RemindersList : SlackMessage { }

    public class ReminderResponse : MessageResponse
    {
        public Reminder reminder { get; set; }
    }

    public class RemindersResponse : MessageResponse
    {
        public Reminder[] reminders { get; set; }
    }
}

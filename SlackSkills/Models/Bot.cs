// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

using System;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

namespace SlackSkills
{
    public class Bot
    {
        public string  id      { get; set; }
        public string  name    { get; set; }
        public bool?   deleted { get; set; }
        public int?    updated { get; set; }
        public string  app_id  { get; set; }
        public string? team_id { get; set; }
        public string? user_id { get; set; }
        public Icons   icons   { get; set; }

        public string bot_user_id      { get; set; }
        public string bot_access_token { get; set; }
    }
}

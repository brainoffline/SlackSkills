﻿using System;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

namespace SlackSkills
{
    public class Team
    {
        public string? id     { get; set; }
        public string? name   { get; set; }
        public string? domain { get; set; }
    }

    public class TeamName
    {
        public string  team_id { get; set; }
        public string? name    { get; set; }
    }
}

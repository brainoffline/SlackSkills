// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized

using System;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

namespace SlackSkills
{
    public class UserProfile : IProfileIcons
    {
        public string  first_name              { get; set; }
        public string  last_name               { get; set; }
        public string  real_name               { get; set; }
        public string  real_name_normalized    { get; set; }
        public string  display_name            { get; set; }
        public string  display_name_normalized { get; set; }
        public string  team                    { get; set; }
        public string  email                   { get; set; }
        public string  skype                   { get; set; }
        public string  status_emoji            { get; set; }
        public string  status_text             { get; set; }
        public int?    status_expiration       { get; set; }
        public string  two_factor_type         { get; set; }
        public string  phone                   { get; set; }
        public string  avatar_hash             { get; set; }
        public string  image_original          { get; set; }
        public string? tz                      { get; set; }
        public string? tz_label                { get; set; }
        public int?    tz_offset               { get; set; }
        public string image_24                { get; set; }
        public string image_32                { get; set; }
        public string image_48                { get; set; }
        public string image_72                { get; set; }
        public string image_192               { get; set; }
        public string image_512               { get; set; }
    }
}

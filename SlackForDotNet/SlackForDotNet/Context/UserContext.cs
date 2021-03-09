using System;

namespace SlackForDotNet.Context
{
    public class UserContext
    {
        public string       UserId  { get; }
        public User         User    { get; }
        public UserProfile? Profile { get; set; }

        public UserContext(string userId, User user, UserProfile? profile)
        {
            UserId = userId;
            User   = user;
        }
    }
}

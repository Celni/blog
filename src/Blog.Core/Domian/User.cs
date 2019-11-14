using System.Collections.Generic;

namespace Blog.Core.Domian
{
    public class User : BaseEntity
    { 
        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public string AvatarPath { get; private set; }

        public ICollection<Comment> Comments { get; private set; }

        private User() { }

        public User(string firstName, string lastName, string avatarPath)
        {
            FirstName = firstName;
            LastName = lastName;
            AvatarPath = avatarPath;
            Comments = new List<Comment>();
        }
    }
}
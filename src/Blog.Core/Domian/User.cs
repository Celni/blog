using System.Collections.Generic;

namespace Blog.Core.Domian
{
    public class User : BaseEntity<string>
    { 
        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public ICollection<Comment> Comments { get; private set; }

        private User() { }

        public User(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
            Comments = new List<Comment>();
        }
    }
}
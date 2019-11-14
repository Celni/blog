using System.Collections.Generic;

namespace Blog.Core.Domian
{
    public class Post : BaseEntity
    {
        public User Author { get; private set; }

        public string Content { get; private set; }

        public ICollection<Comment> Comments { get; private set; }

        public bool IsActive { get; set; }
        private Post() { }

        public Post(User author, string content)
        {
            Author = author;
            Content = content;
            Comments = new List<Comment>();
            IsActive = false;
        }
    }
}
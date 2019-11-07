using System;

namespace Blog.Core.Domian
{
    public class Comment : BaseEntity<ulong>
    {
        public string Text { get; private set; }

        public DateTime Date { get; private set; }

        public User Author { get; private set; }

        public Comment Parent { get; private set; }

        private Comment() { }

        public Comment(User author, string text, DateTime date, Comment parent)
        {
            Author = author;
            Text = text;
            Date = date;
            Parent = parent;
        }
    }
}
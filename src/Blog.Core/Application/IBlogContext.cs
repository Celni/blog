using Blog.Core.Domian;
using Microsoft.EntityFrameworkCore;

namespace Blog.Core.Application
{
    public interface IBlogContext
    {
        DbSet<User> Users { get; set; }

        DbSet<Comment> Comments { get; set; }

        DbSet<Post> Posts { get; set; }
    }
}
using System;
using Blog.Core.Application;
using Blog.Core.Domian;
using Microsoft.EntityFrameworkCore;

namespace Blog.Persistence
{
    public class BlogContext : DbContext, IBlogContext
    {
        public BlogContext(DbContextOptions options) : base(options) {}
        public DbSet<User> Users { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Post> Posts { get; set; }
    }
}

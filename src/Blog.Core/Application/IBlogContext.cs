using System.Threading;
using System.Threading.Tasks;
using Blog.Core.Domian;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Blog.Core.Application
{
    public interface IBlogContext
    {
        DbSet<User> Users { get; set; }

        DbSet<Comment> Comments { get; set; }

        DbSet<Post> Posts { get; set; }

        DatabaseFacade Database { get; }

        int SaveChanges();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Blog.Web.Data
{
    public class IdentityContext : IdentityDbContext<IdentityUser<long>, IdentityRole<long>, long, IdentityUserClaim<long>, IdentityUserRole<long>, IdentityUserLogin<long>, IdentityRoleClaim<long>, IdentityUserToken<long>>
    {
        public IdentityContext(DbContextOptions<IdentityContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SequenceHiLo);
            builder.UseHiLo();

            builder.Entity<IdentityUser<long>>().ToTable("User", "identity");
            builder.Entity<IdentityRole<long>>().ToTable("Role", "identity");
            builder.Entity<IdentityUserClaim<long>>().ToTable("UserClaim", "identity");
            builder.Entity<IdentityRoleClaim<long>>().ToTable("RoleClaim", "identity");

            builder.Entity<IdentityUserLogin<long>>().ToTable("UserLogin", "identity");
            builder.Entity<IdentityUserRole<long>>().ToTable("UserRole", "identity");
            builder.Entity<IdentityUserToken<long>>().ToTable("UserToken", "identity");
        }
    }
}

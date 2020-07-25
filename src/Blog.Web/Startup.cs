using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Core.Application;
using Blog.Persistence;
using Blog.Web.Areas;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Blog.Web.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Blog.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddEntityFrameworkNpgsql().AddDbContext<IdentityContext>(options =>
            {
                options.UseNpgsql(
                    Configuration.GetConnectionString("IdentityConnection"));
            });

            services.AddIdentity<IdentityUser<long>, IdentityRole<long>>()
                .AddEntityFrameworkStores<IdentityContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddGitHub(o =>
                {
                    o.ClientId = Configuration["GitHub:ClientId"];
                    o.ClientSecret = Configuration["GitHub:ClientSecret"];
                    o.SaveTokens = true;
                }).AddCookie();


            services.AddDbContext<IBlogContext, BlogContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("BlogConnection")));

            services.AddLogging();

            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser<long>>>();
            services.AddSingleton<WeatherForecastService>();

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });


            services.ConfigureApplicationCookie(options =>
            {
                options.LogoutPath = $"/LogOut";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseForwardedHeaders();
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });

            InitializeDatabase(app);
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
            var logger = app.ApplicationServices.GetService<ILogger<Startup>>();

            DatabaseFacade db;

            try
            {
                db = scope.ServiceProvider.GetRequiredService<IdentityContext>().Database;
                db.Migrate();
            }
            catch (Exception e)
            {
                logger.LogCritical(e, "Context - {db} error migrate", "IdentityContext");
            }


            try
            {
                db = scope.ServiceProvider.GetRequiredService<IBlogContext>().Database;
                db.Migrate();
            }
            catch (Exception e)
            {
                logger.LogCritical(e, "Context - {db} error migrate", "BlogContext");
            }

        }
    }
}

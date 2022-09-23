using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography;
using DistSysAcw.Auth;
using DistSysAcw.Models;

namespace DistSysAcw
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)      // Called at runtime.
        {
            services.AddDbContext<UserContext>();
            services.AddControllers(options =>
            {
                options.AllowEmptyInputInBodyModelBinding = true;
            });
            services.AddHttpContextAccessor();
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "CustomAuthentication";
            })
            .AddScheme<AuthenticationSchemeOptions, CustomAuthenticationHandler>
                    ("CustomAuthentication", options => { });
            services.AddTransient<IAuthorizationHandler, 
                CustomAuthorizationHandler>();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();        
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

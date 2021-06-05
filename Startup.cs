using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AspNetSample.Models;
using Microsoft.EntityFrameworkCore;
using AspNetSample.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace AspNetSample
{
    public class Startup
    {
    private IConfiguration _configuration;

    // Notice we are using Dependency Injection here
    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddIdentity<IdentityUser, IdentityRole>().
                    AddEntityFrameworkStores<AppDbContext>();
        services.AddDbContextPool<AppDbContext>(options =>
            options.UseSqlServer(_configuration.GetConnectionString("EmployeeDBConnection"))
            );
        services.AddMvc();
        services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
    {
        
        if (!env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseStatusCodePagesWithRedirects("/Error/{0}");
        }

        /*DefaultFilesOptions obj = new DefaultFilesOptions();
        obj.DefaultFileNames.Clear();
        obj.DefaultFileNames.Add("myfile.html");
        app.UseDefaultFiles(obj);*/
        app.UseStaticFiles();
        app.UseAuthentication();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

         /*app.Run(async (context) =>
        {
            await context.Response.WriteAsync("apprun");
        });*/
    }
    }
}

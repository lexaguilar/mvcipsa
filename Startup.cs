using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using mvcIpsa.Data;
using mvcIpsa.DbModelIPSA;
using mvcIpsa.Services;
using mvcIpsa.DbModel;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace mvcIpsa
{
    using Microsoft.AspNetCore.Http.Features;
    using Microsoft.AspNetCore.Localization;
    using Microsoft.Extensions.FileProviders;
    using mvcIpsa.Extensions;
    using System.Globalization;

    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            _hostingEnvironment = environment;
        }

        public IConfiguration Configuration { get; }
        IHostingEnvironment _hostingEnvironment;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<IPSAContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("CustomConnection")));
            services.AddDbContext<DBIPSAContext>(options =>
               options.UseNpgsql(Configuration.GetConnectionString("DBIPSAConnection")));

            services.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit = int.MaxValue;
            });

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                   .AddCookie(options =>
                   {
                       options.LoginPath = "/Account/LogIn";
                       options.LogoutPath = "/Account/LogOut";
                   });
            services.AddAuthorization(o =>
            {
                o.AddPolicy("Admin", p =>
                {
                    p.RequireAssertion(rol =>
                    {
                        return rol.User.Claims.Any(c => c.Type == "roles" && c.Value.Split(',').Any(x => Convert.ToInt32(x) == (int)Roles.Administrador));
                    });
                });
            });
            services.AddAuthorization(o =>
            {
                o.AddPolicy("Admin,User", p =>
                {
                    p.RequireAssertion(rol =>
                    {
                        return rol.User.Claims.Any(c => c.Type == "roles" && c.Value.Split(',').Any(x => Convert.ToInt32(x) == (int)Roles.Administrador || Convert.ToInt32(x) == (int)Roles.Usuario));
                    });
                });
            });
            services.AddAuthorization(o =>
            {
                o.AddPolicy("Admin,Reportes", p =>
                {
                    p.RequireAssertion(rol =>
                    {
                        return rol.User.Claims.Any(c => c.Type == "roles" && c.Value.Split(',').Any(x => Convert.ToInt32(x) == (int)Roles.Administrador || Convert.ToInt32(x) == (int)Roles.Reportes));
                    });
                });
            });
           
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(19466);

            services.AddMvc().AddJsonOptions(o => o.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver());
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.AddSingleton<IFileProvider>(_hostingEnvironment.WebRootFileProvider);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //if (env.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            //else
            {
                //app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            
            app.UseAuthentication();
            app.UseRequestLocalization();
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

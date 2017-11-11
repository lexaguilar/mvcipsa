﻿using System;
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
using mvcIpsa.Models;
using mvcIpsa.Services;
using mvcIpsa.DbModel;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace mvcIpsa
{
    using mvcIpsa.Extensions;
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<IPSAContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("CustomConnection")));

            //services.AddIdentity<ApplicationUser, IdentityRole>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>()
            //    .AddDefaultTokenProviders();

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
                        return rol.User.Claims.Any(c => c.Type == "roles" && c.Value.Split(',').Any(x => Convert.ToInt32(x) == 1));
                    });
                });
            });
            services.AddAuthorization(o =>
            {
                o.AddPolicy("Admin,User", p =>
                {
                    p.RequireAssertion(rol =>
                    {
                        return rol.User.Claims.Any(c => c.Type == "roles" && c.Value.Split(',').Any(x => Convert.ToInt32(x) == 1 || Convert.ToInt32(x) == 2));
                    });
                });
            });
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //if (env.IsDevelopment())
            {
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

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

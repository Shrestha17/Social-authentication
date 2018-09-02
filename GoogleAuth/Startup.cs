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
using GoogleAuth.Data;
using GoogleAuth.Models;
using GoogleAuth.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace GoogleAuth
{
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

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication().AddGoogle(googleOptions =>
            {
                googleOptions.SignInScheme = "ExternalCookie";               
                googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
                googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];

            });
            services.AddAuthentication().AddFacebook(facebookOptions =>
            {
                facebookOptions.SignInScheme = "ExternalCookie";
                facebookOptions.ClientId = Configuration["Authentication:Facebook:ClientId"];
                facebookOptions.ClientSecret = Configuration["Authentication:Facebook:ClientSecret"];


                //facebookOptions.CallbackPath ="/Account/ExternalLoginCallback";
            });


            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie("ExternalCookie");

            //     services.AddAuthentication()
            //.AddCookie(options =>
            //{
            //    options.ForwardDefault = CookieAuthenticationDefaults.AuthenticationScheme;
            //    options.ForwardChallenge = OpenIdConnectDefaults.AuthenticationScheme;
            //    options.LoginPath = "/Account/Unauthorized/";
            //    options.AccessDeniedPath = "/Account/Forbidden/";

            //});


            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.Use(async (HttpContext context, Func<Task> next) =>
            {
                //do work before the invoking the rest of the pipeline       

                await next.Invoke(); //let the rest of the pipeline run

                //do work after the rest of the pipeline has run     
            });





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

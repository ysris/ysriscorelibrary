using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Stripe;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Text;
using ysriscorelibrary.Interfaces;
using YsrisCoreLibrary.Dal;
using YsrisCoreLibrary.Interfaces;
using YsrisCoreLibrary.Middlewares;
using YsrisCoreLibrary.Models;
using YsrisCoreLibrary.Services;

namespace YsrisCoreLibrary.Abstract
{
    /// <summary>
    /// Common Startup implementation
    /// </summary>
    public abstract class AbstractStartup
    {
        public IHostingEnvironment _env;
        public IConfiguration Configuration { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="configuration"></param>
        public AbstractStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public AbstractStartup(IHostingEnvironment env)
        {
            _env = env;
            var builder = new ConfigurationBuilder().SetBasePath(env.ContentRootPath).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        /// <summary>
        // Called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme) //The default bearer need to be this one for making JWT token working (fair enough)
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;

                    cfg.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = Configuration["Tokens:Issuer"],
                        ValidAudience = Configuration["Tokens:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"]))
                    };
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme); // Needed by the cookie auth
            services
                .AddAuthorization(options =>
                {
                    options.AddPolicy("All", p => { p.RequireAuthenticatedUser(); p.RequireClaim(ClaimTypes.Role, new List<string> { "Administrator", "User", "Coach" }); p.Build(); });
                    options.AddPolicy("Administrator", p => { p.RequireAuthenticatedUser(); p.RequireClaim(ClaimTypes.Role, "Administrator"); p.Build(); });
                    options.AddPolicy("User", p => { p.RequireAuthenticatedUser(); p.RequireClaim(ClaimTypes.Role, "User"); p.Build(); });
                    options.AddPolicy("Coach", p => { p.RequireAuthenticatedUser(); p.RequireClaim(ClaimTypes.Role, "Coach"); p.Build(); });
                    options.AddPolicy("CompanyAdministrator", p => { p.RequireAuthenticatedUser(); p.RequireClaim(ClaimTypes.Role, "CompanyAdministrator"); p.Build(); });
                })
                .AddMemoryCache()
                .AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.Converters.Add(new StringEnumConverter { CamelCaseText = false });
                });
            services
                .AddSwaggerGen(c =>
                {
                    // Register the Swagger generator, defining one or more Swagger documents
                    c.SwaggerDoc("v1", new Info
                    {
                        Version = "v1",
                        Title = $"{Configuration.GetValue<string>("Data:AppName")} API",
                        Description = "Default API access",
                        TermsOfService = "None",
                        Contact = new Contact { Name = "Yoann Magli - Ysris", Email = "yoann@ysris.ch", Url = "https://www.ysris.ch" },
                        License = new License { Name = "Ysris Stack", Url = "https://ysris.ch/license" }
                    });
                    c.IncludeXmlComments(Path.Combine(_env.ContentRootPath, "CodeDoc.xml")); // Set the comments path for the Swagger JSON and UI.
                })
                .AddDistributedMemoryCache() // Adds a default in-memory implementation of IDistributedCache
                .AddSession()
                .AddHangfire(x => x.UseMemoryStorage());
            
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<SessionHelperService<ICustomer>>();
            services.AddSingleton<MailHelperService>();
            services.AddScoped<IStorageService, LocalFileSystemStorageService>();
            services.AddSingleton<EncryptionService>();
            services.AddTransient<ViewRenderService>();
            services.AddSingleton<SlackService>();
        }

        /// <summary>
        /// called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IDatabaseSyncDal databaseSyncDal)
        {
            app.UseAuthentication();

            loggerFactory
                .AddConsole(LogLevel.Debug)
                .AddDebug(LogLevel.Debug)
                .AddFile($"Logs/" + Configuration.GetValue<string>("Data:AppName") + "-{Date}.txt", LogLevel.Debug)
                ;

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseHangfireServer().UseHangfireDashboard();

            app.UseStatusCodePages(async context =>
            {
                context.HttpContext.Response.ContentType = "text/json";
                await context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(new { context.HttpContext.Response.StatusCode }));
            });

            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = context =>
                {
                    context.Context.Response.Headers.Add("Cache-Control", "no-cache, no-store");
                    context.Context.Response.Headers.Add("Expires", "-1");
                }
            });

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseSession(); // IMPORTANT: This session thing MUST go before UseMvc()
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));
            app.UseMvc(routes => { routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}"); });
        }
    }
}
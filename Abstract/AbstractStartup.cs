using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ysriscorelibrary.Interfaces;
using YsrisCoreLibrary.Dal;
using YsrisCoreLibrary.Helpers;
using YsrisCoreLibrary.Middlewares;
using YsrisCoreLibrary.Services;

namespace YsrisCoreLibrary.Abstract
{
    public abstract class AbstractStartup
    {

        public IHostingEnvironment _env;

        public AbstractStartup(IConfiguration configuration)
        {
            this.Configuration = configuration;

        }

        public AbstractStartup(IHostingEnvironment env)
        {
            _env = env;
            var builder = new ConfigurationBuilder().SetBasePath(env.ContentRootPath).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
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
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme) //And THIS to keep the cookie auth
            ;


            services.AddAuthorization(options =>
            {
                options.AddPolicy("All", p =>
                {
                    p.RequireAuthenticatedUser();
                    p.RequireClaim(ClaimTypes.Role, new List<string> { "Administrator", "User", "Coach" });
                    p.Build();
                });
                options.AddPolicy("Administrator", p =>
                {
                    p.RequireAuthenticatedUser();
                    p.RequireClaim(ClaimTypes.Role, "Administrator");
                    p.Build();
                });
                options.AddPolicy("User", p =>
                {
                    p.RequireAuthenticatedUser();
                    p.RequireClaim(ClaimTypes.Role, "User");
                    p.Build();
                });
                options.AddPolicy("Coach", p =>
                {
                    p.RequireAuthenticatedUser();
                    p.RequireClaim(ClaimTypes.Role, "Coach");
                    p.Build();
                });
                //CompanyAdministrator
            });


            services.AddMemoryCache();

            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter { CamelCaseText = false });
            });






            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = $"{Configuration.GetValue<string>("Data:AppName")} API",
                    Description = "Default API access",
                    TermsOfService = "None",
                    Contact = new Contact { Name = "Yoann Magli - Ysris", Email = "yoann@ysris.ch", Url = "https://www.ysris.ch" },
                    License = new License { Name = "Ysris Stack", Url = "https://ysris.ch/license" }
                });

                // Set the comments path for the Swagger JSON and UI.
                c.IncludeXmlComments(Path.Combine(_env.ContentRootPath, "CodeDoc.xml"));
            });






            services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache
            services.AddSession();

            services.AddHangfire(x => x.UseMemoryStorage());


            // (O_O) Add the singletons here
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<SessionHelperService>();
            services.AddSingleton<MailHelperService>();
            services.AddSingleton<IStorageService, LocalFileSystemStorageService>();

            services.AddSingleton<EncryptionService>();
            services.AddTransient<CustomerDal>();
            services.AddTransient<ViewRenderService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseAuthentication();

            loggerFactory
                .AddConsole(LogLevel.Debug)
                .AddDebug(LogLevel.Debug)
                .AddFile($"Logs/" + Configuration.GetValue<string>("Data:AppName") + "-{Date}.txt", LogLevel.Debug)
                ;

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseHangfireServer();
            app.UseHangfireDashboard();

            app.UseStatusCodePages(async context =>
            {
                context.HttpContext.Response.ContentType = "text/json";
                await context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(new { context.HttpContext.Response.StatusCode }));
            });

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseSession(); // IMPORTANT: This session thing MUST go before UseMvc()
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));
            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });



            // Everyday at 4AM UTC (5AM GVA)
            //RecurringJob.AddOrUpdate(() => dal.SyncAll(), Cron.Daily(4));
        }
    }
}

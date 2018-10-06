using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using System;
using System.IO;
using Swashbuckle.AspNetCore.Swagger;
using System.Reflection;
using Syzoj.Api.Data;
using Syzoj.Api.Models;
using Microsoft.AspNetCore.Identity;
using Syzoj.Api.Services;
using Newtonsoft.Json.Serialization;
using Syzoj.Api.Problems;
using Syzoj.Api.Mvc;
using System.Linq;
using Syzoj.Api.Problemsets;
using Syzoj.Api.Events;

namespace Syzoj.Api
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
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());
            services.Configure<ApiBehaviorOptions>(options => {
                options.InvalidModelStateResponseFactory = context =>
                    new BadRequestObjectResult(new CustomResponse<object>() {
                        Success = false,
                        Errors = context.ModelState.SelectMany(kv =>
                            kv.Value.Errors.Select(e => new ActionError() {
                                Message = $"{kv.Key}: {e.ErrorMessage}",
                            })
                        ),
                        Result = null,
                    });
            });

            services.AddSignalR();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new Info {
                    Title = "SYZOJ next generation API server",
                    Version = "v1",
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddSingleton<IConnectionMultiplexer>(s => {
                return ConnectionMultiplexer.Connect(Configuration.GetConnectionString("Redis"));
            });

            services.AddSingleton<CacheManager>();

            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = Configuration.GetConnectionString("RedisCache");
                options.InstanceName = "syzoj:session:";
            });

            services.AddSession();

            services.AddDbContext<ApplicationDbContext>(options => 
                options.UseNpgsql(Configuration.GetConnectionString("Database")));
            // services.AddIdentityCore<ApplicationUser>()
            services.AddAuthentication(options => {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies(options => { });
            services.ConfigureApplicationCookie(options => {
                options.Cookie.Name = "SYZOJSESS";
            });
            services.AddIdentityCore<ApplicationUser>(options => {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
                 .AddEntityFrameworkStores<ApplicationDbContext>()
                 .AddDefaultUI()
                 .AddDefaultTokenProviders();
            
            services.AddSingleton<IConnection>(s => {
                var factory = new ConnectionFactory();
                Uri uri;
                if(!Uri.TryCreate(Configuration.GetConnectionString("RabbitMQ"), UriKind.Absolute, out uri))
                    throw new ArgumentException("Invalid RabbitMQ connection string");
                factory.Uri = uri;
                factory.DispatchConsumersAsync = true;
                return factory.CreateConnection();
            });
            services.AddSingleton<ILegacySyzojJudger, LegacySyzojJudger>();

            services.AddSingleton<IAsyncFileStorageProvider>(s => {
                var section = Configuration.GetSection("FileSystem");
                var type = section.GetValue<string>("Type");
                switch(type)
                {
                    case "Local":
                        var path = section.GetValue<string>("Path");
                        var secret = section.GetValue<string>("Secret");
                        return new LocalFileStorageProvider(path, secret);
                    default:
                        throw new ArgumentException("Invalid FileSystemType in configuration file.");
                }
            });

            services.AddSingleton<ProblemResolverDictionary>();
            services.AddScoped<IProblemResolverService, ProblemResolverService>();
            services.AddSingleton<ProblemsetResolverDictionary>();
            services.AddScoped<IProblemsetResolverService, ProblemsetResolverService>();
            services.AddSingleton<IEventService, EventService>();

            services.AddSingleton<IProblemResolverProvider, Problems.Standard.StandardProblemResolverProvider>();
            services.AddSingleton<Problems.Standard.StandardProblemJudger>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SYZOJ API");
            });

            app.UseSession();
            app.UseAuthentication();
            app.UseSignalR(route => {
                route.MapHub<Problems.Standard.StandardJudgeHub>("/standard-judge");
            });
            app.UseMvc();

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}

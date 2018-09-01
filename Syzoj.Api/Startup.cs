using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using StackExchange.Redis;
using Syzoj.Api.Data;
using Syzoj.Api.Models;
using Syzoj.Api.Utils;
using Syzoj.Api.Services;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using Syzoj.Api.Models.Responses;
using Syzoj.Api.Filters;

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
            services.AddDbContext<ApplicationDbContext>(options => 
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(opt => opt.SerializerSettings.ContractResolver
                    = new DefaultContractResolver());
            
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressConsumesConstraintForFormFileParameters = true;
                options.SuppressInferBindingSourcesForParameters = true;
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddSingleton<IConnection>(s => {
                var factory = new ConnectionFactory();
                Configuration.GetSection("RabbitMQ").Bind(factory);
                return factory.CreateConnection();
            });

            services.AddSingleton<ILegacyRunnerManager, LegacyRunnerManager>();

            services.AddSingleton<IConnectionMultiplexer>(s => {
                return ConnectionMultiplexer.Connect(Configuration.GetValue<string>("Redis"));;
            });

            services.AddTransient<SessionMiddleware>();

            services.AddHttpContextAccessor();

            services.AddScoped<ISessionManager, SessionManager>();

            services.AddHttpClient();

            services.AddScoped<IBlobManager, BlobManager>();

            services.AddScoped<IProblemResolverProvider, ProblemResolverProvider>();
            services.AddScoped<LegacyProblemResolver>();
            services.AddTransient<LegacyProblemController>();

            services.AddScoped<IProblemSetServiceProvider, ProblemSetServiceProvider>();

            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "SYZOJ Web API",
                    Description = "The SYZOJ next generation API server, in ASP.NET Core",
                    License = new License
                    {
                        Name = "GNU Affero General Public License v3.0",
                    }
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                c.OperationFilter<RequiresLoginOperationFilter>();
                c.OperationFilter<ValidateModelOperationFilter>();
                c.OperationFilter<SessionHeaderOperationFilter>();
            });
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
                // Application should not use HSTS
                // app.UseHsts();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SYZOJ Web API");
            });

            // 应用不应该使用 HTTPS
            // app.UseHttpsRedirection();

            app.UseMiddleware<SessionMiddleware>();

            app.UseMvc();

            // Warm it up so it can receive task results
            app.ApplicationServices.GetService<ILegacyRunnerManager>();
        }
    }
}

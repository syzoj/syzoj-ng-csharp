﻿using System;
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
                    = new DefaultContractResolver());;

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

            services.AddScoped<IProblemManager, ProblemManager>();

            services.AddScoped<IBlobManager, BlobManager>();
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

            // 应用不应该使用 HTTPS
            // app.UseHttpsRedirection();

            app.UseMiddleware<SessionMiddleware>();

            app.UseMvc();

            // Warm it up so it can receive task results
            app.ApplicationServices.GetService<ILegacyRunnerManager>();
        }
    }
}

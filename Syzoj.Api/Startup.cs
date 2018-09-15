﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using SharpFileSystem;
using System;
using SharpFileSystem.FileSystems;
using System.IO;
using Swashbuckle.AspNetCore.Swagger;
using System.Reflection;

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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

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

            services.AddDbContext<AppDbContext>(options => 
                options.UseNpgsql(Configuration.GetConnectionString("Database")));
            
            services.AddSingleton<IConnection>(s => {
                var factory = new ConnectionFactory();
                Uri uri;
                if(!Uri.TryCreate(Configuration.GetConnectionString("RabbitMQ"), UriKind.Absolute, out uri))
                    throw new ArgumentException("Invalid RabbitMQ connection string");
                factory.Uri = uri;
                return factory.CreateConnection();
            });

            services.AddSingleton<IFileSystem>(s => {
                var section = Configuration.GetSection("FileSystem");
                var type = section.GetValue<string>("Type");
                switch(type)
                {
                    case "Memory":
                        return new MemoryFileSystem();
                    case "Local":
                        var path = section.GetValue<string>("Path");
                        return new PhysicalFileSystem(path);
                    default:
                        throw new ArgumentException("Invalid FileSystemType in configuration file.");
                }
            });

            services.AddSingleton<ProblemsetManagerProvider>();
            services.AddSingleton<ProblemParserProvider>();
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

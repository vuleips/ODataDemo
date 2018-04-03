using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ODataServer.Extensions;
using ODataServer.Models;
using System;

namespace ODataServer
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
            services.AddMvc();
            services.AddOData();

            // Adds a default in-memory implementation of IDistributedCache.
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.Name = ".ODataServer.Demo";
                options.Cookie.HttpOnly = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSession();

            var builder = new ODataConventionModelBuilder();
            builder.EntitySet<Material>("Materials");
            builder.EntityType<Material>().Filter("Name");
            builder.EntityType<Material>().OrderBy("Name");
            app.UseMvc(routes =>
                routes.MapODataServiceRoute("ODataRoute", null, builder.GetEdmModel(),
                    pathHandler: new PathAndSlashEscapeODataPathHandler(), 
                    routingConventions: ODataRoutingConventions.CreateDefault())
            );
        }
    }
}

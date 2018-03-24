﻿using Dotnettency;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Sample.EFCore
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMultiTenancy<Tenant>((multiTenancyOptions) =>
            {
                multiTenancyOptions
               // .DistinguishTenantsWith<CookieTenantDistinguisherFactory>()
                    .InitialiseTenant<TenantShellFactory>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMultitenancy<Tenant>((options) =>
            {
                options.UsePerTenantContainers();

            });

            app.Run(async (context) =>
            {
                var tenantTask = context.RequestServices.GetRequiredService<Task<Tenant>>();
                var tenant = await tenantTask;

                await context.Response.WriteAsync("Hello: " + tenant.Name);
            });
        }
    }
}

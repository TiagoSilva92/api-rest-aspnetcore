using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RCN.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.ResponseCompression;
using System;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace RCN.API
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
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddJsonOptions(opt=>{
                    opt.JsonSerializerOptions.IgnoreNullValues = true;
                })
                .AddXmlSerializerFormatters();

            services.AddDbContext<ProdutoContexto>(opt =>
                opt.UseInMemoryDatabase(databaseName: "produtoInMemory")
                   .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

            services.AddTransient<IProdutoRepository, ProdutoRepository>();

            services.AddVersionedApiExplorer(opt=>
            {
                opt.GroupNameFormat = "'v'VV";
                opt.SubstituteApiVersionInUrl = true;
            });

            services.AddApiVersioning();

            services.AddResponseCaching();

             services.AddResponseCompression(opt =>
            {
                //opt.Providers.Add<GzipCompressionProvider>();
                opt.Providers.Add<BrotliCompressionProvider>();
                opt.EnableForHttps = true;
            });

            services.AddSwaggerGen(c =>
            {
                var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

                foreach (var item in provider.ApiVersionDescriptions)
                {
                    c.SwaggerDoc(item.GroupName, new OpenApiInfo 
                    { 
                        Title = $"API de Produtos {item.ApiVersion}", 
                        Version = item.ApiVersion.ToString()
                    });
                }
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
           
            app.UseHttpsRedirection();

            app.UseResponseCaching();

            app.UseResponseCompression();

             app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
               foreach (var item in provider.ApiVersionDescriptions)
                {
                   c.SwaggerEndpoint($"/swagger/{item.GroupName}/swagger.json", item.GroupName);
                }

                c.RoutePrefix = string.Empty;
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });           
        }
    }
}

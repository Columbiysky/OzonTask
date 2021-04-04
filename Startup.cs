using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using OzonTask.Models;
using Microsoft.Extensions.Configuration;

namespace OzonTask
{
    public class Startup
    {
        public IConfiguration Configuration { get; private set; }
        private string ConnectionString { get; set; }
        private User DefaultUser { get; set; }
        public Startup(IConfiguration configuration )
        {
            Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<UsersContext>(provider => new UsersContext(ConnectionString));
            services.AddTransient<EmailsContext>(provider => new EmailsContext(ConnectionString, DefaultUser));
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            ConnectionString = Configuration.GetSection("DataBase").GetSection("DefaultConnectionString").Value;
            DefaultUser = new User();
            Configuration.GetSection("User").Bind(DefaultUser);
            app.UseDeveloperExceptionPage();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

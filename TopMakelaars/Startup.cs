using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TopMakelaars
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(600);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddTransient<IDataAggregator, DataAggregator>();
            services.AddTransient<IApiClient, ApiClient>();
            services.AddHttpClient("FundaPartnerApi",
            client =>
            {
                client.BaseAddress = new Uri(Configuration["FundaPartnerApiUrl"] + "json/" + Configuration["FundaPartnerApiKey"] + "/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));              
            });
        }


        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env
                             , Microsoft.AspNetCore.Hosting.IApplicationLifetime applicationLifetime)
        {

            app.UseExceptionHandler("/Error");

            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();

            app.UseMvc();

            applicationLifetime.ApplicationStopping.Register(() =>
            {
                var fundaPartnerApiDataPath = Path.Combine(Path.GetTempPath(), "fundaPartnerApiData");
                Directory.Delete(fundaPartnerApiDataPath);
            });
        }
    }
}

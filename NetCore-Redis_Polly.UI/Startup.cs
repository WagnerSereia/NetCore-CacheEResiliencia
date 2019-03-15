using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCore_Redis_Polly.UI.ResilienciaPolly;
using Polly;
using System.Reflection;

namespace NetCore_Redis_Polly.UI
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            //Aplicando resiliencia com Polly, CENARIO 1
            //services.AddPollyPolicies();

            //Aplicando resiliencia com Polly, CENARIO 1

            services.AddHttpClient(Configuration.GetSection("API:NomeApi").Get<string>(), client =>
            {                
                client.BaseAddress = new Uri(Configuration.GetSection("API:UrlBase").Get<string>());
                client.DefaultRequestHeaders.Add("User-Agent", "resilient-api");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddTransientHttpErrorPolicy(policy => policy.RetryAsync(GetConfigurationValue("RetryCount")))
            .AddTransientHttpErrorPolicy(policy => policy.CircuitBreakerAsync(GetConfigurationValue("HandledEventsAllowedBeforeBreaking"), TimeSpan.FromSeconds(GetConfigurationValue("DurationOfBreakInSeconds"))))
            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(GetConfigurationValue("TimeOutInSeconds")));

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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc();
        }

        private readonly IDictionary<string, int> _defaultValues = new Dictionary<string, int>()
        {
            { "RetryCount", 3},
            { "HandledEventsAllowedBeforeBreaking", 5},
            { "DurationOfBreakInSeconds", 30},
            { "TimeOutInSeconds", 30}
        };
        private int GetConfigurationValue(string key)
        {
            var applicationConfigurationSection = Configuration.GetSection("PollyConfiguration");

            if (string.IsNullOrEmpty(applicationConfigurationSection.GetSection(key).Value))
                return _defaultValues[key];
            else
                return Convert.ToInt32(applicationConfigurationSection.GetSection(key).Value);
        }
    }
}

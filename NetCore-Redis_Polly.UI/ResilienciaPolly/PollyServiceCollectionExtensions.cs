using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Bulkhead;
using Polly.CircuitBreaker;
using Polly.Registry;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace NetCore_Redis_Polly.UI.ResilienciaPolly
{
    public static class PollyServiceCollectionExtensions
    {
        public static IServiceCollection AddPollyPolicies(this IServiceCollection services)
        {
            var registry = services.AddPolicyRegistry();

            registry.AddBasicRetryPolicy();

            services.AddScoped<IPollyController, PollyController>();

            return services;
        }
               
        #region CircuitBreaker COMENTADO
        /*
        public static void AddPollyCircuitBreaker(this IServiceCollection services, IConfiguration configuration)
        {
            #region Configuracao do HttpClient
            HttpClient httpClient = new HttpClient()
            {
                BaseAddress = new Uri(configuration.GetSection("RedisCache").Get<string>())
            };
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            #endregion

            #region Configuraçao da Policy para CircuitBreaker
            CircuitBreakerPolicy<HttpResponseMessage> breakerPolicy =
                Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                    .CircuitBreakerAsync(2, TimeSpan.FromSeconds(60), OnBreak, OnReset, OnHalfOpen);
            #endregion

            #region Configuração da Policy para Bulkhead
            BulkheadPolicy<HttpResponseMessage> bulkheadIsolationPolicy =
                Policy.BulkheadAsync<HttpResponseMessage>(2, 4, OnBulkheadRejectedAsync);
            #endregion

            #region Injecao de Dependencia das policies
            var _myRegistry = new PolicyRegistry();
            services.AddSingleton<HttpClient>(httpClient);
            services.AddSingleton(_myRegistry);
            services.AddSingleton<CircuitBreakerPolicy<HttpResponseMessage>>(breakerPolicy);
            services.AddSingleton<BulkheadPolicy<HttpResponseMessage>>(bulkheadIsolationPolicy);
            #endregion
        }

        private static Task OnBulkheadRejectedAsync(Context context)
        {
            Debug.WriteLine("OnBulkheadRejectedAsync executed");
            return Task.CompletedTask;
        }

        private static void OnBreak(DelegateResult<HttpResponseMessage> delegateResult, TimeSpan timeSpan)
        {
            Debug.WriteLine($"Connection break: {delegateResult.Result}, {delegateResult.Result}");
        }

        private static void OnHalfOpen()
        {
            Debug.WriteLine("Connection Half Open");
        }

        private static void OnReset()
        {
            Debug.WriteLine("Connection Reset");
        }
        */
        #endregion        
    }
}
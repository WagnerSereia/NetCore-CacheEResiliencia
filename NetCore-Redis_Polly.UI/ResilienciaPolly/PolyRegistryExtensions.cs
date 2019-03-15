using System;
using System.Net.Http;
//using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Registry;
using Polly.Timeout;
using Polly.Wrap;

namespace NetCore_Redis_Polly.UI.ResilienciaPolly
{
    public static class PolyRegistryExtensions
    {        
        public static IPolicyRegistry<string> AddBasicRetryPolicy(this IPolicyRegistry<string> policyRegistry)
        {            
            var retryPolicy = Policy
                .Handle<Exception>()
                .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .Or<TimeoutRejectedException>()
                .WaitAndRetryAsync(3, retryCount => TimeSpan.FromSeconds(Math.Pow(2, retryCount)), (result, timeSpan, retryCount, context) =>
                {
                    if (!context.TryGetLogger(out var logger)) return;

                    context.TryGetValue("url", out var url);

                    #region ApplicationInsights
                    //var telemetry = new TelemetryClient();
                    //if (result.Exception != null)
                    //{
                    //    telemetry.TrackException(new Exception($"An exception occurred on retry {retryCount} for {context.PolicyKey} " +
                    //                                           $"on URL {url}"));
                    //}
                    //else
                    //{
                    //    telemetry.TrackException(new Exception($"A non success code {(int)result.Result.StatusCode} " +
                    //                                           $"was received on retry {retryCount} for {context.PolicyKey}. " +
                    //                                           $"Will retry in {Math.Pow(2, retryCount)} seconds on URL {url}"));
                    //}
                    #endregion
                })
                .WithPolicyKey(PolicyNames.BasicRetry);

            policyRegistry.Add(PolicyNames.BasicRetry, retryPolicy);
            
            return policyRegistry;
            
        }
    }
}
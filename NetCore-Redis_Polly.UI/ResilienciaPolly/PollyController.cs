using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetCore_Redis_Polly.UI.ResilienciaPolly;
using Newtonsoft.Json;
using Polly;
using Polly.Registry;
using Polly.Retry;
using Polly.Timeout;

namespace NetCore_Redis_Polly.UI.ResilienciaPolly
{
    public class PollyController : IPollyController
    {
        private string _baseUri;

        private readonly IReadOnlyPolicyRegistry<string> _policyRegistry;
        private readonly Guid RequesterId = Guid.NewGuid();
        private readonly ILogger _logger;

        readonly TimeoutPolicy _timeoutPolicy;
        readonly RetryPolicy<HttpResponseMessage> _httpRetryPolicy;
        public PollyController(IReadOnlyPolicyRegistry<string> policyRegistry)
        {
            #region Configuracao API
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            _baseUri = config.GetSection("API:UrlBase").Get<string>();
            #endregion


            #region Policies da Resiliencia
            _policyRegistry = policyRegistry;
            _timeoutPolicy = Policy.TimeoutAsync(3);

            _httpRetryPolicy =
                Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                    .Or<TimeoutRejectedException>()
                    .RetryAsync(6);
            #endregion
        }

        public async Task<HttpResponseMessage> PostToApi(dynamic data, string apiUrl)
        {
            var client = new HttpClient();
            var byteData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));

            var content = new ByteArrayContent(byteData);

            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var retryPolicy = _policyRegistry.Get<IAsyncPolicy<HttpResponseMessage>>(PolicyNames.BasicRetry)
                              ?? Policy.NoOpAsync<HttpResponseMessage>();

            var context = new Context($"GetSomeData-{Guid.NewGuid()}", new Dictionary<string, object>
                {
                    { PolicyContextItems.Logger, _logger }, { "url", apiUrl }
                });

            var retries = 0;
            
            var response = await retryPolicy.ExecuteAsync((ctx) =>
            {
                client.DefaultRequestHeaders.Remove("retries");
                client.DefaultRequestHeaders.Add("retries", new[] { retries++.ToString() });

                var baseUrl = _baseUri;
                //if (string.IsNullOrWhiteSpace(baseUrl))
                //{
                //    var uri = Request.GetUri();
                //    baseUrl = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;
                //}

                var isValid = Uri.IsWellFormedUriString(apiUrl, UriKind.Absolute);

                return client.PostAsync(isValid ? $"{baseUrl}{apiUrl}" : $"{baseUrl}", content);
            }, context);

            content.Dispose();

            return response;

        }

        public async Task<HttpResponseMessage> GetToApi(string apiUrl)
        {
            var _httpClient = new HttpClient();

            HttpResponseMessage response =
               await _httpRetryPolicy.ExecuteAsync(() =>
                        _timeoutPolicy.ExecuteAsync(
                            async token => await _httpClient.GetAsync($"{_baseUri}{apiUrl}", token), CancellationToken.None));
            return response;

            #region Comentado
            /*
            var client = new HttpClient();
            var byteData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));

            var content = new ByteArrayContent(byteData);

            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var retryPolicy = _policyRegistry.Get<IAsyncPolicy<HttpResponseMessage>>(PolicyNames.BasicRetry)
                              ?? Policy.NoOpAsync<HttpResponseMessage>();

            var context = new Context($"GetSomeData-{Guid.NewGuid()}", new Dictionary<string, object>
                {
                    { PolicyContextItems.Logger, _logger }, { "url", apiUrl }
                });

            var retries = 0;

            var response = await retryPolicy.ExecuteAsync((ctx) =>
            {
                client.DefaultRequestHeaders.Remove("retries");
                client.DefaultRequestHeaders.Add("retries", new[] { retries++.ToString() });

                var baseUrl = _baseUri;
                //if (string.IsNullOrWhiteSpace(baseUrl))
                //{
                //    var uri = Request.GetUri();
                //    baseUrl = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;
                //}

                var isValid = Uri.IsWellFormedUriString(baseUrl + apiUrl, UriKind.Absolute);

                return client.GetAsync(isValid ? $"{baseUrl}{apiUrl}" : $"{baseUrl}");
            }, context);

            content.Dispose();

            return response;
            */
            #endregion
        }
    }
}
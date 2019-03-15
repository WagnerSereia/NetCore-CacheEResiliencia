using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NetCore_Redis_Polly.Api.CacheRedis
{
    public class ServiceConfigurationRedis:IConfigurationRedis
    {
        private readonly IConfiguration _configuration;

        public ServiceConfigurationRedis(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public ConfigurationRedis getConfiguration()
        {
            var config = _configuration.GetSection("RedisCache").Get<ConfigurationRedis>();            
            return config;
        }
    }

    public static class DependencyInjectionRedisConfiguration
    {
        public static void AddRedisConfiguration(this IServiceCollection services)
        {
            services.AddSingleton<IConfigurationRedis, ServiceConfigurationRedis>();
        }
    }
}

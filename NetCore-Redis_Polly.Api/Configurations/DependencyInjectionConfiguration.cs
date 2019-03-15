using Microsoft.Extensions.DependencyInjection;
using NetCore_Redis_Polly.Domain.Configuracoes;

namespace NetCore_Redis_Polly.Api.Configurations
{
    public static class DependencyInjectionConfiguration
    {
        public static void AddDIConfiguration(this IServiceCollection services)
        {
            NativeInjector.RegisterServices(services);
        }
    }
}

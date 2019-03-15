using Microsoft.Extensions.DependencyInjection;
using NetCore_Redis_Polly.Domain.Interface;
using NetCore_Redis_Polly.Domain.Repositorio;

namespace NetCore_Redis_Polly.Domain.Configuracoes
{
    public class NativeInjector
    {
        public static void RegisterServices(IServiceCollection services)
        {                 
            //Utilizado Singleton, somente para fins didaticos - NAO UTILIZAR SINGLETON PARA REPOSITORIO
            services.AddSingleton<IRepositorioUsuario, UsuarioRepositorio>();
        }
    }
}

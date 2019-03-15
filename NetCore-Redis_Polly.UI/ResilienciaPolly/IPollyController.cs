using System.Net.Http;
using System.Threading.Tasks;

namespace NetCore_Redis_Polly.UI.ResilienciaPolly
{
    public interface IPollyController
    {
        Task<HttpResponseMessage> PostToApi(dynamic data, string apiUrl);
        Task<HttpResponseMessage> GetToApi(string apiUrl);
    }
}
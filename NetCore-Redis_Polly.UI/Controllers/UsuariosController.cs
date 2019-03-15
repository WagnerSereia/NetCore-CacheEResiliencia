using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NetCore_Redis_Polly.UI.ResilienciaPolly;
using NetCore_Redis_Polly.UI.ViewModels;
using Newtonsoft.Json;
using Polly.Registry;

namespace NetCore_Redis_Polly.UI.Controllers
{
    [Route("Usuarios")]
    public class UsuariosController : Controller
    {
        private readonly IHttpClientFactory _clientHttp;
        private readonly string _nomeApi;
        private const string _urlApi = "usuarios/";
        private readonly HttpClient client;

        //private readonly IPollyController _pollyController;
        //public UsuariosController(IPollyController pollyController)
        //{
        //    _pollyController = pollyController;
        //}
        public UsuariosController(IHttpClientFactory clientHttp)
        {
            _clientHttp = clientHttp;
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            _nomeApi = config.GetSection("Api:NomeApi").Get<string>();
            client = _clientHttp.CreateClient(_nomeApi);
        }

        public async Task<IActionResult> Index()
        {
            var url = _urlApi + "listar-usuarios";
            //HttpResponseMessage response = _pollyController.GetToApi(url).Result;
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {                
                var usuarios = JsonConvert.DeserializeObject<IEnumerable<UsuarioViewModel>>(await response.Content.ReadAsStringAsync());
                return Ok(usuarios);
            }

            return StatusCode((int)response.StatusCode, response.Content.ReadAsStringAsync());
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using NetCore_Redis_Polly.Api.CacheRedis;
using NetCore_Redis_Polly.Domain.Entidade;
using NetCore_Redis_Polly.Domain.Interface;
using Newtonsoft.Json;

namespace NetCore_Redis_Polly.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioUsuario _repositorioUsuario;
        private readonly IDistributedCache _distributedCache;
        private readonly IConfigurationRedis _configurationRedis;
        static int _requestCount = 0;

        public UsuariosController(
            IMapper mapper, 
            IRepositorioUsuario repositorioUsuario, 
            IDistributedCache distributedCache,
            IConfigurationRedis configurationRedis)
        {
            _mapper = mapper;
            _repositorioUsuario = repositorioUsuario;
            _distributedCache = distributedCache;
            _configurationRedis = configurationRedis;
        }
        // GET api/values
        [HttpGet]
        [Route("listar-usuarios")]
        public async Task<IActionResult> ListarUsuarios()
        {
            _requestCount++;
            Debug.WriteLine($"listar-usuarios, {_requestCount}ª vez");
                        

            //if (_requestCount % 6 != 0)
            {
                await Task.Delay(3000); //1 Minuto
                //return StatusCode((int)HttpStatusCode.RequestTimeout, "TimeOut...");
            }
            

            var cacheKey = "usuarios";
            var cacheUsuarios = _distributedCache.GetString(cacheKey);
            if (!string.IsNullOrEmpty(cacheUsuarios))
            {
                var usuarios = JsonConvert.DeserializeObject<List<Usuario>>(cacheUsuarios);
                return Ok(usuarios);
            }
            else
            {
                var usuarios = _mapper.Map<List<Usuario>>(_repositorioUsuario.ListarUsuario());

                var distributedCacheOption = new DistributedCacheEntryOptions();
                distributedCacheOption.SetAbsoluteExpiration(TimeSpan.FromMinutes(_configurationRedis.getConfiguration().tempoCacheMinutos));
                _distributedCache.SetString(cacheKey, JsonConvert.SerializeObject(usuarios), distributedCacheOption);
                return Ok(usuarios);
            }
        }

        [HttpPost]
        [Route("criar-usuario")]
        public IActionResult Post(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                _repositorioUsuario.AdicionarUsuario(_mapper.Map<Usuario>(usuario));
                return Created($"/api/usuario{usuario.IdUsuario}", usuario);
            }
            return BadRequest(ModelState);
        }
               

        [HttpDelete]
        [Route("remover-usuario/{id}")]
        public IActionResult Delete([FromRoute]Guid id)
        {
            if (ModelState.IsValid)
            {
                _repositorioUsuario.RemoverUsuario(id);
                return Ok();
            }
            return BadRequest(ModelState);
        }
    }
}

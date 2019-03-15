using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore_Redis_Polly.Api.CacheRedis
{
    public interface IConfigurationRedis
    {
        ConfigurationRedis getConfiguration();
    }
}

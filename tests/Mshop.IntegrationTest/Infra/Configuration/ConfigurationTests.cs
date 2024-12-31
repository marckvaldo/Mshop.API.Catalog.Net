using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.IntegrationTests.Infra.Configuration
{
    public static class ConfigurationTests
    {
        public const string ConnectionStrings = "Server=localhost;Port=3308;Database=mshop;User id=root;Password=mshop;Convert Zero Datetime=True";
        public const string RedisEndpoint = "localhost:8378";
        public const string RedisUser = "";
        public const string RedisPasseword = "";
    }
}

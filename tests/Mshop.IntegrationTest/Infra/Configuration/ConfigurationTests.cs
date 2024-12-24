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
        public static string DataBase()
        {
            Random random = new Random();
            return  random.Next(0, 999).ToString();   
        }

        public const string ConnectionStrings = "Server=localhost;Port=3308;Database=mshop;User id=root;Password=mshop;Convert Zero Datetime=True";
        public const string RedisEndpoint = "localhost:8378";
        public const string RedisUser = "";
        public const string RedisPasseword = "";

        public static string NewConnection()
        {
            return ConnectionStrings.Replace("Database=mshop", $"Database=mshop_{DataBase()}");
        }

    }
}

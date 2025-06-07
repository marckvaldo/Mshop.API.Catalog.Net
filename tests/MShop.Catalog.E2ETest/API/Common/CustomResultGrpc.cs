using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.Catalog.E2ETests.API.Common
{
    public class CustomResultGrpc<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public CustomResultGrpc(bool success, string message, T data)
        {
            Success = success;
            Message = message;
            Data = data;
        }
    }
}

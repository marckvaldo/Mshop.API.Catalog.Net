using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Mshop.Catalog.E2ETests.Base.Clients
{
    public class GrpcClient
    {
        protected HttpClient _grpcClient;
        protected GrpcChannel _channel;

        public GrpcClient(HttpClient httpClient)
        {
            _grpcClient = httpClient;

            _channel = GrpcChannel.ForAddress(_grpcClient.BaseAddress, new GrpcChannelOptions
            {
                HttpClient = _grpcClient
            });
        }

        public async Task<(Metadata?, TOutPut?)> SimpleCall<TClient, TRequest, TOutPut>(
            Func<TClient, TRequest, Task<TOutPut>> method,
            TRequest request)
          where TClient : class
          where TRequest : class
          where TOutPut : class

        {
            // Criação do cliente gRPC e adionar o canal o mesmo que 
            //var client = new ProductProto.ProductProtoClient(channel);
            var client = Activator.CreateInstance(typeof(TClient), _channel) as TClient;

            if (client == null)
                throw new InvalidOperationException($"Não foi possível criar o cliente do tipo {typeof(TClient).Name}.");

            try
            {
                // Fazendo a chamada gRPC
                var output = await method(client, request);

                // Retorno bem-sucedido com os metadados (se precisar)
                return (null, output);
            }
            catch (RpcException ex)
            {
                // Retorna os metadados no caso de erro
                return (ex.Trailers, null);
            }
        }


        public async Task<(Metadata?, List<TOutPut>?)> ServerStreamingCall<TClient, TRequest, TOutPut>(
           Func<TClient, TRequest, IAsyncEnumerable<TOutPut>> method,
           TRequest request)
            where TClient : class
            where TOutPut : class
            where TRequest : class
        {
            var client = Activator.CreateInstance(typeof(TClient), _channel) as TClient;
            if (client == null)
                throw new InvalidOperationException($"Não foi possível criar o cliente do tipo {typeof(TClient).Name}.");

            try
            {
                var outputs = new List<TOutPut>();
                await foreach (var output in method(client, request))
                {
                    outputs.Add(output);
                }

                return (null, outputs);
            }
            catch (RpcException ex)
            {
                return (ex.Trailers, null);
            }
        }

    }
}

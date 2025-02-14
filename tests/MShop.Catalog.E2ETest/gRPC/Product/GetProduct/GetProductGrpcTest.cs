using Microsoft.Extensions.DependencyInjection;
using Mshop.Core.Data;
using Mshop.gRPC.Catalog.Protos;
using Mshop.Infra.Data.Context;
using Mshop.Infra.Data.Interface;
using Mshop.Infra.Data.Repository;
using Mshop.Infra.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.Catalog.E2ETests.gRPC.Product.GetProduct
{
    public class GetProductGrpcTest : GetProductGrpcTestFixture
    {
        private ICategoryRepository _categoryRepository;
        private IProductRepository _productRepository;
        private RepositoryDbContext _dbContext;
        private IUnitOfWork _unitOfWork;

        public GetProductGrpcTest() : base() {

            _categoryRepository = _serviceProvider.GetRequiredService<ICategoryRepository>();
            _productRepository = _serviceProvider.GetRequiredService<IProductRepository>();
            _dbContext = _serviceProvider.GetRequiredService<RepositoryDbContext>();
            _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();
        }

        [Fact(DisplayName = nameof(GetProductGRPCShoudReturnNotFoudProduct))]
        [Trait("EndToEnd/GRPC", "Product - Endpoints")]

        public async Task GetProductGRPCShoudReturnNotFoudProduct()
        {
            var request = new GetProductRequest { Id = "c56a4180-65aa-42ec-a945-5fd21dec0538" };

            //aqui eu estou definindo a função que será chamada no método SimpleCall
            async Task<CustomerResultGrpc> GrpcCall(ProductProto.ProductProtoClient client, GetProductRequest request)
            {
                return await client.GetProductByIdAsync(request);
            }


            var (metadata, result) = await _grpcClient.SimpleCall<ProductProto.ProductProtoClient, GetProductRequest, CustomerResultGrpc>(
                                       
                                       //async (client, request) => await client.GetProductByIdAsync(request),
                                       GrpcCall, // aqui estou ingetando a função que será chamada
                                       request // O parâmetro de requisição
                                    );


            Assert.False(result.Success);
            Assert.True(result.Errors.Count() > 0 ? true : false);
        }

        [Fact(DisplayName = nameof(GetProductGRPCShoudReturnProduct))]
        [Trait("EndToEnd/GRPC", "Product - Endpoints")]

        public async Task GetProductGRPCShoudReturnProduct()
        {
            var category = FakerCategory();
            var product = FakerProduct(category);
            await _productRepository.Create(product, CancellationToken.None);
            await _unitOfWork.CommitAsync(CancellationToken.None);

            var request = new GetProductRequest { Id = product.Id.ToString() };

            //aqui eu estou definindo a função que será chamada no método SimpleCall
            async Task<CustomerResultGrpc> GrpcCall(ProductProto.ProductProtoClient client, GetProductRequest request)
            {
                return await client.GetProductByIdAsync(request);
            }


            var (metadata, result) = await _grpcClient.SimpleCall<ProductProto.ProductProtoClient, GetProductRequest, CustomerResultGrpc>(

                                       //async (client, request) => await client.GetProductByIdAsync(request),
                                       GrpcCall, // aqui estou ingetando a função que será chamada
                                       request // O parâmetro de requisição
                                    );

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.False(result.Errors.Count() > 0 ? true : false);
            Assert.True(result.Data.Id == product.Id.ToString());
            Assert.True(result.Data.Name == product.Name);
            Assert.True(result.Data.Description == product.Description);
            Assert.True(result.Data.Price == (float) product.Price);
            Assert.True(result.Data.CategoryId == product.CategoryId.ToString());
            Assert.True(result.Data.Category == product.Category.Name);
            Assert.True(result.Data.Stock == (float) product.Stock);
        }
    }
}

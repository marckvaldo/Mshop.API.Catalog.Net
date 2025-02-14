using Mshop.gRPC.Catalog.Protos;
using Grpc.Core;
using MediatR;
using CoreMessage = Mshop.Core.Message;

using Mshop.Application.UseCases.Product.GetProduct;
using Google.Protobuf.WellKnownTypes;

namespace Mshop.gRPC.Catalog.Services
{
    public class ProductService : ProductProto.ProductProtoBase
    {
        private IMediator _mediator;
        private CoreMessage.INotification _notification;
        public ProductService(IMediator mediator, CoreMessage.INotification notification)
        {
            _mediator = mediator;
            _notification = notification;
        }

        public override async Task<CustomerResultGrpc> GetProductById(GetProductRequest request, ServerCallContext context)
        {
            IsCancelationToken(context);
            IsValidation(request);

            if (!OperationIsValid())
                return Error();

            var result = await _mediator.Send(new GetProductInPut(Guid.Parse(request.Id)));

            if (OperationIsValid())
            {
                var product = result.Data;
                return Sucess(new GetProductReply
                {
                    Id = product.Id.ToString(),
                    Name = product.Name,
                    Description = product.Description,
                    Price = (float)product.Price,
                    Stock = (float)product.Stock,
                    IsActive = product.IsActive,
                    CategoryId = product.CategoryId.ToString(),
                    Category = product.Category.Name
                });
            }

            return Error();

        }




        private bool OperationIsValid()
        {
            return !_notification.HasErrors();
        }

        private CustomerResultGrpc Sucess(GetProductReply result) /*where T : Google.Protobuf.IMessage<T>*/
        {
            return new CustomerResultGrpc
            {
                Success = true,
                //Data = Any.Pack(result),
                Data = result,
                Errors = { }
            };
        }
        
        private CustomerResultGrpc Error()
        {
            return new CustomerResultGrpc
            {
                Success = false,
                Data = null,
                Errors = { _notification.Errors().Select(x => x.Message).ToList() }
            };
        }

        private void IsCancelationToken(ServerCallContext context)
        {
            if (context.CancellationToken.IsCancellationRequested)
                _notification.AddNotifications("Request was canceled");
        }

        private void IsValidation(GetProductRequest request)
        {
            Guid id = Guid.Empty;
            if (!Guid.TryParse(request.Id, out id))
            {
                _notification.AddNotifications("Invalid Id");
            }
        }
    }
}

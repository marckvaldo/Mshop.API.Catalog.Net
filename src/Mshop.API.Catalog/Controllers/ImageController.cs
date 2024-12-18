using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mshop.Application.UseCases.Images.Common;
using Mshop.Application.UseCases.Images.CreateImage;
using Mshop.Application.UseCases.Images.DeleteImage;
using Mshop.Application.UseCases.Images.GetImage;
using Mshop.Application.UseCases.Images.ListImage;

namespace Mshop.API.Catalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : MainController
    {
        private readonly IMediator _mediator;
        /*private readonly IGetImage _getImage;
        private readonly IDeleteImage _deleteImage;
        private readonly ICreateImage _createImage;
        private readonly IListImage _listImage;*/

        public ImageController(
            /*IGetImage getImage,
            IDeleteImage deleteImage,
            ICreateImage createImage,
            IListImage listImage,*/
            IMediator mediator,
            Core.Message.INotification notification
            ) : base(notification)
        {
            /*_getImage = getImage;
            _deleteImage = deleteImage;
            _createImage = createImage;
            _listImage = listImage;*/
            _mediator = mediator;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ImageOutPut>> Image(Guid id, CancellationToken cancellation)
        {
            return CustomResponse(await _mediator.Send(new GetImageInPut(id), cancellation));        
        }

        [HttpGet("list-Images-by-id-production/{productId:guid}")]
        public async Task<ActionResult<ListImageOutPut>> ListImagesByIdProduction(Guid productId, CancellationToken cancellation)
        {
            return CustomResponse(await _mediator.Send(new ListImageInPut(productId), cancellation));
        }

        [HttpPost]        
        public async Task<ActionResult<ListImageOutPut>> Create(CreateImageInPut image, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) CustomResponse(ModelState);
            return CustomResponse(await _mediator.Send(image, cancellationToken));
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ImageOutPut>> Delete(Guid id, CancellationToken cancellationToken)
        {
            return CustomResponse(await _mediator.Send(new DeleteImageInPut(id), cancellationToken));
        }

    }
}


using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mshop.Application.UseCases.Category.Common;
using Mshop.Application.UseCases.Images.Common;
using Mshop.Application.UseCases.Images.CreateImage;
using Mshop.Application.UseCases.Images.DeleteImage;
using Mshop.Application.UseCases.Images.GetImage;
using Mshop.Application.UseCases.Images.ListImage;

namespace Mshop.API.Catalog.Controllers.v1
{
    [ApiVersion(1.0)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ImagesController : MainController
    {
        private readonly IMediator _mediator;

        public ImagesController(
            IMediator mediator,
            Core.Message.INotification notification
            ) : base(notification)
        {
            _mediator = mediator;
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(CategoryModelOutPut), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ImageOutPut>> Images(Guid id, CancellationToken cancellation)
        {
            var result = await _mediator.Send(new GetImageInPut(id), cancellation);
            if (result.Data is null) return CustomResponse(404);
            return CustomResponse(result);
        }

        [HttpGet("list-images-by-productId/{id:guid}")]
        [ProducesResponseType(typeof(CategoryModelOutPut), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ListImageOutPut>> ListImagesByIdProduction(Guid id, CancellationToken cancellation)
        {
            var result = await _mediator.Send(new ListImageInPut(id), cancellation);
            if (result.Data is null) return CustomResponse(404);
            return CustomResponse(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(CategoryModelOutPut), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ListImageOutPut>> Create(CreateImageInPut image, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) CustomResponse(ModelState);
            return CustomResponse(await _mediator.Send(image, cancellationToken),201);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(CategoryModelOutPut), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ImageOutPut>> Delete(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeleteImageInPut(id), cancellationToken);
            if (result.Data is null) return CustomResponse(404);
            return CustomResponse(result);
        }

    }
}


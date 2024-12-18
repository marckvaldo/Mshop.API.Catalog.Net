using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Mshop.Core.Message;
using Mshop.ProductAPI.Extension;

namespace Mshop.API.Catalog.Filter
{
    public class ApiGlobalExceptionFilter : IExceptionFilter
    {
        private readonly IHostEnvironment _env;
        private readonly INotification _notification;
        public ApiGlobalExceptionFilter(IHostEnvironment env, INotification notification)
        {
            _env = env;
            _notification = notification;
        }

        public void OnException(ExceptionContext context)
        {
            var details = new ProblemDetails();
            var exception =  context.Exception;

            if(_env.IsDevelopment())
            {
                details.Extensions.Add("StackTrace", exception.StackTrace);
            }

            //_notification.AddNotifications(exception.Message);
            Notify(exception.Message);

            /*if(exception is EntityValidationException)
            {
                var ex = exception as EntityValidationException;
                details.Title = "One or more validation or erros ocurred";
                details.Status = StatusCodes.Status422UnprocessableEntity;
                details.Detail = ex!.Message;
                details.Type = "UnprocessableEntity";
            }
            else if(exception is ApplicationValidationException)
            {
                var ex = exception as EntityValidationException;
                details.Title = "One or more validation or erros ocurred";
                details.Status = StatusCodes.Status400BadRequest;
                details.Detail = ex!.Message;
                details.Type = "UnprocessableEntity";
            }
            else
            {
                details.Title = "An unexpected error ocurred";
                details.Status = StatusCodes.Status400BadRequest;
                details.Detail = exception!.Message;
                details.Type = "UnprocessableEntity";
            }*/


            details.Title = "An unexpected error ocurred";
            details.Status = StatusCodes.Status400BadRequest;
            details.Detail = exception!.Message;
            details.Type = "UnprocessableEntity";
            context.HttpContext.Response.StatusCode = (int) details.Status;
            
            context.Result = new ObjectResult(ExtensionResponse.Error(_notification.Errors().Select(x => x.Message).ToList()));  
            context.ExceptionHandled = true;
            
        }

        public void Notify(string message)
        {
           _notification.AddNotifications(message);
        }

       
    }
}

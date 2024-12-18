using Microsoft.Extensions.DependencyInjection;
using Mshop.Application.Service;
using Mshop.Domain.Contract.Services;
using CoreMessage = Mshop.Core.Message;

namespace Mshop.Application
{
    public static class ServiceResgistrationExtensions
    {
        public static IServiceCollection AddUseCase(this IServiceCollection services)
        {
            services.AddScoped<IBuildCacheCategory, BuildCacheCategory>();
            services.AddScoped<IBuildCacheImage, BuildCacheImage>();
            services.AddScoped<IBuildCacheProduct, BuildCacheProduct>();

            services.AddScoped<CoreMessage.INotification, CoreMessage.Notifications>();
            services.AddScoped<IStorageService, StorageService>();

            services.AddMediatR(x => x.RegisterServicesFromAssemblies(typeof(ServiceResgistrationExtensions).Assembly));

            
            

            return services;
        }
    }
}

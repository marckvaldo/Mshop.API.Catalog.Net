using Microsoft.Extensions.DependencyInjection;
using Mshop.Domain.Contract.Services;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Data.Interface;

namespace Mshop.Application.Service
{
    public class BuildCacheImage : IBuildCacheImage
    {
        //private IImagesCacheRepository _imageCacheRepository;
        //private IImageRepository _imageRepository;

        private IServiceProvider _serviceProvider;

        public BuildCacheImage(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Handle()
        {
            using var scope = _serviceProvider.CreateScope();
            var imageRepository = scope.ServiceProvider.GetRequiredService<IImageRepository>();
            var imageCacheRepository = scope.ServiceProvider.GetRequiredService<IImagesCacheRepository>();

            var expirationDate = DateTime.UtcNow.AddHours(1);

            var images = await imageRepository.Filter(x => x.FileName != "");

            foreach (var image in images)
            {
                await imageCacheRepository.Create(image, expirationDate, CancellationToken.None);
            }
        }
    }
}

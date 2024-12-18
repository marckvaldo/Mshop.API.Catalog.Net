using Mshop.Domain.Contract.Services;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Data.Interface;

namespace Mshop.Application.Service
{
    public class BuildCacheImage : IBuildCacheImage
    {
        private IImagesCacheRepository _imageCacheRepository;
        private IImageRepository _imageRepository;

        public BuildCacheImage(IImagesCacheRepository iamgeCacheRepository, IImageRepository imageRepository)
        {
            _imageCacheRepository = iamgeCacheRepository;
            _imageRepository = imageRepository;
        }

        public async void Handle()
        {
            var expirationDate = DateTime.UtcNow.AddHours(1);

            var images = await _imageRepository.Filter(x => x.FileName != "");

            foreach (var image in images)
            {
                await _imageCacheRepository.AddImage(image, expirationDate, CancellationToken.None);
            }
        }
    }
}

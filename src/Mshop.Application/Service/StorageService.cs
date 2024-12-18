using Mshop.Domain.Contract.Services;

namespace Mshop.Application.Service
{
    public class StorageService : IStorageService
    {
        public Task<bool> Delete(string FileName)
        {
            return Task.Run<bool>(() => true);
        }

        public Task<string> Upload(string FileName, Stream FileStreang)
        {
            return Task.Run<string>(() => FileName);
        }
    }
}

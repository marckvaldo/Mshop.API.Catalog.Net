namespace Mshop.Domain.Contract.Services
{
    public interface IStorageService
    {
        Task<string> Upload(string FileName, Stream FileStreang);

        Task<bool> Delete(string FileName);
    }
}

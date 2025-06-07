namespace Mshop.Infra.Cache.Interface
{
    public interface IConfigCacheRespository
    {
        Task<DateTime?> GetExpirationDate();

        Task SetExpirationDate(DateTime expirationDate);
        Task ClearCache();
    }
}

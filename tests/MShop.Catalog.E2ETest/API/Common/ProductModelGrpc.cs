namespace Mshop.Catalog.E2ETests.API.Common
{
    public record ProductModelGrpc(Guid Id, string Description, string Name, decimal Price, bool IsPromotion, Guid CategoryId, string Category, string? Thumb);
}

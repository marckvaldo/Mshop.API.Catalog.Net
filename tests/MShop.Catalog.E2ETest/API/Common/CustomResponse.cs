namespace Mshop.Catalog.E2ETests.API.Common
{
    public class CustomResponse<TResult>
    {

        public TResult Data { get; set; }

        public bool Success { get; set; }

        public CustomResponse(TResult data, bool success)
        {
            Data = data;
            Success = success;
        }

    }
}

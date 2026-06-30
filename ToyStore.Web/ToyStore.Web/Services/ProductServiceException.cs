namespace ToyStore.Web.Services
{
    public class ProductServiceException : Exception
    {
        public ProductServiceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

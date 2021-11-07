namespace eShopWebApi.ViewModels
{
    public class InternalServerErrorResponse
    {
        public InternalServerErrorResponse()
        {
        }

        public InternalServerErrorResponse(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}

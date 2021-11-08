namespace eShopWebApi.ViewModels
{
    public class ServerErrorResponse
    {
        public ServerErrorResponse()
        {
        }

        public ServerErrorResponse(string message)
        {
            Reason = message;
        }

        public string Reason { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
    }
}

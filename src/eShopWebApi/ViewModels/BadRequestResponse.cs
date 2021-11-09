using System.Collections.Generic;

namespace eShopWebApi.ViewModels
{
    public class BadRequestResponse
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public string TraceId { get; set; }
        public Dictionary<string, string[]> Errors { get; set; }
    }
}

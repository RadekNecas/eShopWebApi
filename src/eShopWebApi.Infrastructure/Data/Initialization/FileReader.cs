using System.IO;

namespace eShopWebApi.Infrastructure.Data.Initialization
{
    public class FileReader : IFileReader
    {
        public string ReadAllFile(string file) => File.ReadAllText(file);
    }
}

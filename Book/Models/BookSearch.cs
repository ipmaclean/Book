using Google.Apis.Books.v1;
using Google.Apis.Books.v1.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book.Models
{
    public class BookSearch
    {

        public static BooksService service = new BooksService(
            new BaseClientService.Initializer
            {
                ApplicationName = "BookSearch",
                ApiKey = GetApiKey(),
            });

        public static async Task<Volumes> Search(string searchTerm, int count, int offset)
        {
            var query = service.Volumes.List(searchTerm);
            query.MaxResults = count;
            query.StartIndex = offset;

            var result = await query.ExecuteAsync();
            if (result != null && result.Items != null)
            {
                return result;
            }
            return null;
        }

        public static async Task<Volume> Search(string id)
        {
            var query = service.Volumes.Get(id);

            var result = await query.ExecuteAsync();
            if (result != null && result != null)
            {
                return result;
            }
            return null;
        }

        public static string GetApiKey()
        {
            var path = @"C:\Users\iainp\OneDrive\Github\Book\Book\Content\ApiKey.txt";

            var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            var sr = new StreamReader(fs, Encoding.UTF8);

            return sr.ReadToEnd();
        }
    }
}
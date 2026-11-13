using System.Security.Cryptography.X509Certificates;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Transport;

namespace main;

class Program
{
    static void Main(string[] args)
    {
        // the password is not kept in the source; it is the ELASTIC_PASSWORD value from the .env file
        var password = Environment.GetEnvironmentVariable("ELASTIC_PASSWORD");
        if (string.IsNullOrEmpty(password))
        {
            Console.Error.WriteLine("Set the ELASTIC_PASSWORD environment variable to the value from your .env file.");
            Environment.ExitCode = 1;
            return;
        }

        var settings = new ElasticsearchClientSettings(new Uri("https://localhost:9200"))
            .Authentication(new BasicAuthentication("elastic", password))
            // this disables the cert verification so that we can use our self-signed one
            .ServerCertificateValidationCallback((_, _, _, _) => true);
        
        var client = new ElasticsearchClient(settings); 
        
        var tweet = new Tweet 
        {
            Id = 1,
            User = "stevejgordon",
            PostDate = new DateTime(2009, 11, 15),
            Message = "Trying out the client, so far so good?"
        };

        var response = client.IndexAsync(tweet, t => t.Index("my-tweet-index"), new CancellationToken()).Result; 

        if (response.IsValidResponse) 
        {
            Console.WriteLine($"Index document with ID {response.Id} succeeded."); 
        } else 
        {
            Console.WriteLine($"Something went wrong: {response.ApiCallDetails}"); 
            Console.WriteLine("\n\n\n-----------------------------------------------"); 
            Console.WriteLine(response.DebugInformation); 
        }
        
        
        var response2 = client.GetAsync<Tweet>(1, idx => idx.Index("my-tweet-index")).Result; 

        if (response2.IsValidResponse)
        {
            var tweet2 = response2.Source; 
            Console.WriteLine(tweet2.Message);
        }
        
        var request = new SearchRequest("my-tweet-index") 
        {
            From = 0,
            Size = 10,
            Query = new TermQuery { Field = "user", Value = "stevejgordon" }
        };
        var response3 = client.SearchAsync<Tweet>(request).Result; 

        if (response3.IsValidResponse)
        {
            var twee3 = response3.Documents.FirstOrDefault(); 
            Console.WriteLine("query: " + twee3?.Message);
        }
    }
}
    
    public class Tweet
    {
        public int Id { get; set; } 
        public string User { get; set; }
        public DateTime PostDate { get; set; }
        public string Message { get; set; }
    }
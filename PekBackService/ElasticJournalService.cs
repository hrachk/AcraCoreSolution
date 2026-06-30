using Elastic.Clients.Elasticsearch;
using System;
using System.Threading.Tasks;

namespace PekBackService
{
    public class ElasticJournalService
    {
        private readonly ElasticsearchClient _client;

        public ElasticJournalService(ElasticsearchClient client)
        {
            _client = client;
        }

        public async Task LogAsync(PekJournalDocument doc)
        {
            var response =  await _client.IndexAsync(doc, idx => idx.Index($"pek-journal-{DateTime.UtcNow.ToString("yyyy.MM.dd")}").Id(doc.Id));
            Console.WriteLine(response.DebugInformation);
        }
    }
}
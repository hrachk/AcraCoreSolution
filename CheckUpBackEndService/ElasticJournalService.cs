using Elastic.Clients.Elasticsearch;
using System;
using System.Threading.Tasks;

namespace CheckUpBackEndService
{
    public class ElasticJournalService
    {
        private readonly ElasticsearchClient _client;

        public ElasticJournalService(ElasticsearchClient client)
        {
            _client = client;
        }

        public async Task LogAsync(CheckUpJournalDocument doc)
        {
            try
            {
                var index = $"checkup-journal-{DateTime.UtcNow:yyyy.MM.dd}";
                var response = await _client.IndexAsync(doc, idx => idx.Index(index).Id(doc.Id));
                Console.WriteLine(response.DebugInformation);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.UtcNow} [ElasticJournalService] LogAsync failed: {ex.Message}");
            }
        }
    }
}

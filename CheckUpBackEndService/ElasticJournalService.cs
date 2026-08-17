using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CheckUpBackEndService
{
    public class ElasticJournalService
    {
        private readonly ElasticsearchClient _client;
        private readonly ILogger<ElasticJournalService> _logger;

        public ElasticJournalService(ElasticsearchClient client, ILogger<ElasticJournalService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task LogAsync(CheckUpJournalDocument doc)
        {
            if (doc == null)
            {
                _logger.LogWarning("CheckUpJournalDocument is null – skip Elastic log");
                return;
            }

            try
            {
                var index = $"checkup-journal-{DateTime.UtcNow:yyyy.MM.dd}";
                var response = await _client.IndexAsync(doc, idx => idx.Index(index).Id(doc.Id));

                if (!response.IsValidResponse)
                {
                    _logger.LogError("Elastic index failed for CheckUp journal. EventType={EventType}, UserName={UserName}, Debug={Debug}",
                        doc.EventType, doc.UserName, response.DebugInformation);
                }
                else
                {
                    _logger.LogInformation("CheckUp journal logged. EventType={EventType}, UserName={UserName}, ErrorCode={ErrorCode}",
                        doc.EventType, doc.UserName, doc.ErrorCode);
                }
            }
            catch (Exception ex)
            {
                // Never let Elastic failure break the main request flow
                _logger.LogError(ex, "ElasticJournalService.LogAsync failed. EventType={EventType}, UserName={UserName}",
                    doc.EventType, doc.UserName);
            }
        }
    }
}

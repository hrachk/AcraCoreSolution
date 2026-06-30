using Elastic.Clients.Elasticsearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ElasticService
{
    private readonly ElasticsearchClient _client;

    public ElasticService(ElasticsearchClient client)
    {
        _client = client;
    }

    public async Task IndexAsync<T>(T document)
    {
        var RESPONSE = await _client.IndexAsync(document);
        Console.WriteLine(RESPONSE.DebugInformation);
    }

    public async Task<List<T>> SearchAsync<T>(string field, string value)
    {
        var response = await _client.SearchAsync<T>(s => s
            .Query(q => q.Match(m => m.Field(field).Query(value))));
        return response.Documents.ToList();
    }
}
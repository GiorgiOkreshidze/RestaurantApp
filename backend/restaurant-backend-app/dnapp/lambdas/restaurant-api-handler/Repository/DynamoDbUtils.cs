using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;

namespace Function.Repository;

public static class DynamoDbUtils
{
    public static async Task<List<Document>> ScanDynamoDbTableAsync(AmazonDynamoDBClient client, string? tableName)
    {
        var table = Table.LoadTable(client, tableName);
        var scanFilter = new ScanFilter();
        var search = table.Scan(scanFilter);
        List<Document> documentList = new List<Document>();
        
        do {
            documentList.AddRange(await search.GetNextSetAsync());
        } while (!search.IsDone);

        return documentList;
    }
}
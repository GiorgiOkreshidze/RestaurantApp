using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Function.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Function.Services
{
    public class DynamoDBService : IDynamoDBService
    {
        private readonly IDynamoDBContext _dynamoDBContext;
        private readonly AmazonDynamoDBClient _dynamoDBClient;

        public DynamoDBService()
        {
            _dynamoDBClient = new AmazonDynamoDBClient();
            _dynamoDBContext = new DynamoDBContext(_dynamoDBClient);
        }

    }
}

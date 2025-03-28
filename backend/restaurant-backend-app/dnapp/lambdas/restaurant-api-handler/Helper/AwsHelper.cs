using System;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;

namespace Function.Helper;

public static class AwsHelper
{
    public static SessionDto GetAmazonConfigs()
    {
        var awsCredentials = GetAwsCredentials();
        var dynamoConfig = GetAmazonDynamoDbConfig();

        var result = new SessionDto()
        {
            Config = dynamoConfig,
            Credentials = awsCredentials
        };

        return result;
    }  
    
    private static  AmazonDynamoDBConfig GetAmazonDynamoDbConfig()
    {
        var dynamoConfig = new AmazonDynamoDBConfig()
        {
            RegionEndpoint = RegionEndpoint.EUWest2
        };

        return dynamoConfig;
    }

    private  static SessionAWSCredentials GetAwsCredentials()
    {
        var accessKey = Environment.GetEnvironmentVariable("aws_access_key_id");
        var secretKey = Environment.GetEnvironmentVariable("aws_secret_access_key");
        var sessionToken = Environment.GetEnvironmentVariable("aws_session_token");

        var credentials = new SessionAWSCredentials(accessKey, secretKey, sessionToken);

        return credentials;
    }
}

public class SessionDto
{
    public SessionAWSCredentials? Credentials { get; set; }
    public AmazonDynamoDBConfig? Config { get; set; }
}
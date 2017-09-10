using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Newtonsoft.Json;

namespace DynORM
{
    /// <summary>
    /// Creates a new repository for dynamodb
    /// </summary>
    public class RepositoryFactory
    {
        private static volatile RepositoryFactory _instance;
        private static object _syncRoot = new Object();
        private readonly AmazonDynamoDBClient _dynamoClient;
        private readonly string _enviromentPrefix;

        /// <summary>
        /// Constructor for this factory.
        /// It reads the configuration file and makes a new instance
        /// of AmazonDynamoDBClient with the configurated values in config file
        /// </summary>
        private RepositoryFactory()
        {
            _enviromentPrefix = ConfigReader.Instance.EnviromentPrefix;

            //Set the enviroment prefix
            if (!string.IsNullOrWhiteSpace(_enviromentPrefix))
                if (!_enviromentPrefix.EndsWith("-") && !_enviromentPrefix.EndsWith("_") &&
                    !_enviromentPrefix.EndsWith("."))
                    _enviromentPrefix += "-";
            _enviromentPrefix = _enviromentPrefix.Trim();

            
            //Set credentials
            //If credentials is null, then use credentials from IAM ROLE
            var credentials = ConfigReader.Instance.Credentials;
            if (credentials == null)
            {
                var config = new AmazonDynamoDBConfig();
                config.RegionEndpoint = ConfigReader.Instance.Endpoint;
                _dynamoClient = new AmazonDynamoDBClient();
            }
            else
                _dynamoClient = new AmazonDynamoDBClient(credentials, ConfigReader.Instance.Endpoint);

        }

        /// <summary>
        /// Get the current instance of this factory
        /// </summary>
        public static RepositoryFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                            _instance = new RepositoryFactory();
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Makes a new instance of a repository, using the current configurated values
        /// </summary>
        /// <typeparam name="TModel">Type of the repository</typeparam>
        /// <returns>New instance for the TModel Repository</returns>
        public IRepository<TModel> MakeNew<TModel>() where TModel : class
        {   
            return new Repository<TModel>(_enviromentPrefix, _dynamoClient);
        }

        /// <summary>
        /// Makes a new instance of a repository, using the injected AmazonDynamoDBClient
        /// </summary>
        /// <typeparam name="TModel">Type of the repository</typeparam>
        /// <param name="dynamoClient">Client to connect to DynamoDB Service</param>
        /// <returns>New instance for the TModel Repository</returns>
        public IRepository<TModel> MakeNew<TModel>(AmazonDynamoDBClient dynamoClient) where TModel : class
        {
            return new Repository<TModel>(_enviromentPrefix, dynamoClient);
        }

        /// <summary>
        /// Makes a new instance of a repository, using the injected AmazonDynamoDBClient
        /// </summary>
        /// <typeparam name="TModel">Type of the repository</typeparam>
        /// <param name="dynamoClient">Client to connect to DynamoDB Service</param>
        /// <param name="enviromentPrefix">Enviroment prefix for the TModel table</param>
        /// <returns>New instance for the TModel Repository</returns>
        public IRepository<TModel> MakeNew<TModel>(AmazonDynamoDBClient dynamoClient, string enviromentPrefix) where TModel : class
        {
            return new Repository<TModel>(enviromentPrefix, dynamoClient);
        }
    }
}

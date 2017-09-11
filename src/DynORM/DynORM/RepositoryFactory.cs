using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
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
        private readonly IDictionary<string, object> _tableRequestDictionary;
        private readonly ConfigReader _configReader;

        /// <summary>
        /// Constructor for this factory.
        /// It reads the configuration file and makes a new instance
        /// of AmazonDynamoDBClient with the configurated values in config file
        /// </summary>
        private RepositoryFactory()
        {
            _tableRequestDictionary = new Dictionary<string, object>();
            _configReader = ConfigReader.Instance;
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
            var tableRequestBuilder = GetTableRequestBuilder<TModel>();
            return new Repository<TModel>(_configReader.Credentials, _configReader.Endpoint, tableRequestBuilder);
        }

        /// <summary>
        /// Makes a new instance of a repository, using the injected AmazonDynamoDBClient
        /// </summary>
        /// <typeparam name="TModel">Type of the repository</typeparam>
        /// <param name="credentials">Credentials to connect to DynamoDB Service</param>
        /// <returns>New instance for the TModel Repository</returns>
        public IRepository<TModel> MakeNew<TModel>(AWSCredentials credentials) where TModel : class
        {
            var tableRequestBuilder = GetTableRequestBuilder<TModel>();
            return new Repository<TModel>(credentials, _configReader.Endpoint, tableRequestBuilder);
        }

        /// <summary>
        /// Makes a new instance of a repository, using the injected AmazonDynamoDBClient
        /// </summary>
        /// <typeparam name="TModel">Type of the repository</typeparam>
        /// <param name="credentials">Credentials to connect to DynamoDB Service</param>
        /// <param name="endpoint">Endpoint of the DynamoDB service</param>
        /// <returns>New instance for the TModel Repository</returns>
        public IRepository<TModel> MakeNew<TModel>(AWSCredentials credentials, RegionEndpoint endpoint) where TModel : class
        {
            var tableRequestBuilder = GetTableRequestBuilder<TModel>();
            return new Repository<TModel>(credentials, endpoint, tableRequestBuilder);
        }

        private TableRequestBuilder<TModel> GetTableRequestBuilder<TModel>() where TModel: class
        {
            TableRequestBuilder<TModel> tableRequestBuilder = null;
            var key = typeof(TModel).FullName;
            if (_tableRequestDictionary.ContainsKey(key))
                tableRequestBuilder = (TableRequestBuilder<TModel>)_tableRequestDictionary[key];
            else
            {
                tableRequestBuilder = new TableRequestBuilder<TModel>(_configReader.EnviromentPrefix);
                _tableRequestDictionary.Add(key, tableRequestBuilder);
            }

            return tableRequestBuilder;
        }
    }
}

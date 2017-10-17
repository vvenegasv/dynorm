using System;
using Amazon;
using Amazon.Runtime;
using DynORM.Implementations;
using DynORM.Interfaces;

namespace DynORM.Helpers
{
    /// <summary>
    /// Creates a new repository for dynamodb
    /// </summary>
    public class RepositoryFactory
    {
        private static volatile RepositoryFactory _instance;
        private static object _syncRoot = new Object();
        private readonly ConfigReader _configReader;

        /// <summary>
        /// Constructor for this factory.
        /// It reads the configuration file and makes a new instance
        /// of AmazonDynamoDBClient with the configurated values in config file
        /// </summary>
        private RepositoryFactory()
        {
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
        public IDynoRepo<TModel> MakeNew<TModel>() where TModel : class
        {
            return new DynoRepo<TModel>(_configReader.Credentials, _configReader.Endpoint);
        }

        /// <summary>
        /// Makes a new instance of a repository, using the injected AmazonDynamoDBClient
        /// </summary>
        /// <typeparam name="TModel">Type of the repository</typeparam>
        /// <param name="credentials">Credentials to connect to DynamoDB Service</param>
        /// <returns>New instance for the TModel Repository</returns>
        public IDynoRepo<TModel> MakeNew<TModel>(AWSCredentials credentials) where TModel : class
        {
            return new DynoRepo<TModel>(credentials, _configReader.Endpoint);
        }

        /// <summary>
        /// Makes a new instance of a repository, using the injected AmazonDynamoDBClient
        /// </summary>
        /// <typeparam name="TModel">Type of the repository</typeparam>
        /// <param name="credentials">Credentials to connect to DynamoDB Service</param>
        /// <param name="endpoint">Endpoint of the DynamoDB service</param>
        /// <returns>New instance for the TModel Repository</returns>
        public IDynoRepo<TModel> MakeNew<TModel>(AWSCredentials credentials, RegionEndpoint endpoint) where TModel : class
        {
            return new DynoRepo<TModel>(credentials, endpoint);
        }        
    }
}

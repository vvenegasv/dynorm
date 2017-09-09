using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

namespace DynORM
{
    public class RepositoryFactory
    {
        private static volatile RepositoryFactory _instance;
        private static object _syncRoot = new Object();
        private readonly IDictionary<string, object> _repositories;
        private readonly AmazonDynamoDBClient _dynamoClient;
        private readonly string _enviromentPrefix;

        private RepositoryFactory()
        {
            _repositories = new Dictionary<string, object>();
            _enviromentPrefix = "";
            _dynamoClient = new AmazonDynamoDBClient();
        }

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

        public IRepository<TModel> GetRepository<TModel>() where TModel : class
        {
            var repositoryName = typeof(TModel).FullName;
            if (_repositories.ContainsKey(repositoryName))
                return (IRepository<TModel>) _repositories[repositoryName];

            var repository = new Repository<TModel>(_enviromentPrefix, _dynamoClient);
            _repositories[repositoryName] = repository;
            return repository;
        }
    }
}

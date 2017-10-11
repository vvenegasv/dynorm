using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using DynORM.Helpers;
using System.Reflection;
using Amazon;
using Amazon.Runtime;
using DynORM.Filters;

namespace DynORM
{
    public class Repository<TModel> : IRepository<TModel> where TModel : class
    {
        private readonly AWSCredentials _credentials;        

        internal Repository(AWSCredentials credentials, RegionEndpoint endpoint)
        {
            _credentials = credentials;
        }

        public Task Create(TModel item)
        {
            throw new NotImplementedException();
        }

        public Task Create(TModel item, IFilterBuilder<TModel> condition)
        {
            throw new NotImplementedException();
        }

        public Task Delete(TModel item)
        {
            throw new NotImplementedException();
        }

        public Task Delete(TModel item, IFilterBuilder<TModel> condition)
        {
            throw new NotImplementedException();
        }

        public Task Delete(IFilterBuilder<TModel> condition)
        {
            throw new NotImplementedException();
        }

        public Task<TModel> GetOne<THashKey>(THashKey hashKey)
        {
            throw new NotImplementedException();
        }

        public Task<TModel> GetOne<THashKey, TRangeKey>(THashKey hashKey, TRangeKey rangeKey)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TModel>> QueryByIndex<TIndex>(IFilterBuilder<TIndex> filter) where TIndex : class
        {
            throw new NotImplementedException();
        }

        public Task Update(TModel item)
        {
            throw new NotImplementedException();
        }

        public Task Update(TModel item, IFilterBuilder<TModel> condition)
        {
            throw new NotImplementedException();
        }

        public Task Update(dynamic item, IFilterBuilder<TModel> condition)
        {
            throw new NotImplementedException();
        }

        private AmazonDynamoDBClient GetDynamoDbClient()
        {
            if (_credentials == null)
            {
                var config = new AmazonDynamoDBConfig();
                config.RegionEndpoint = ConfigReader.Instance.Endpoint;
                return new AmazonDynamoDBClient(config);
            }
            
            return new AmazonDynamoDBClient(_credentials, ConfigReader.Instance.Endpoint);
        }
        
    }
}


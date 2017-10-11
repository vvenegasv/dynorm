using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using DynORM.Helpers;
using DynORM.Interfaces;

namespace DynORM.Implementations
{
    public class Repository<TModel> : IRepository<TModel> where TModel : class
    {
        private readonly AWSCredentials _credentials;
        private readonly RegionEndpoint _endpoint;
        private readonly PropertyHelper _propertyHelper;

        internal Repository(AWSCredentials credentials, RegionEndpoint endpoint)
        {
            _credentials = credentials;
            _endpoint = endpoint;
            _propertyHelper = PropertyHelper.Instance;
        }

        public bool IsConsistentRead { get; set; }

        public Task Create(TModel item)
        {
            if(item == null)
                throw new ArgumentNullException(nameof(item));

            var client = GetDynamoDbClient();
            var tableName = _propertyHelper.GetTableName(item);
            //client.PutItemAsync(tableName, )
            return null;
        }

        public Task Create(TModel item, IFilterable<TModel> condition)
        {
            throw new NotImplementedException();
        }

        public Task Delete(TModel item)
        {
            throw new NotImplementedException();
        }

        public Task Delete(TModel item, IFilterable<TModel> condition)
        {
            throw new NotImplementedException();
        }

        public Task Delete(IFilterable<TModel> condition)
        {
            throw new NotImplementedException();
        }

        public Task<TModel> GetOne<THashKey>(THashKey hashKey) where THashKey : class
        {
            throw new NotImplementedException();
        }

        public Task<TModel> GetOne<THashKey, TRangeKey>(THashKey hashKey, TRangeKey rangeKey)
            where THashKey : class
            where TRangeKey : class
        {
            throw new NotImplementedException();
        }

        public IScannable<TModel> Query<THashKey>(THashKey hashKey) where THashKey : class
        {
            throw new NotImplementedException();
        }

        public IScannable<TModel> Query<THashKey>(THashKey hashKey, Dictionary<string, Tuple<object, Type>> lastEvaluatedKey) where THashKey : class
        {
            throw new NotImplementedException();
        }

        public IScannable<TProjection> Query<TIndex, TProjection>(IFilterable<TIndex> filter)
            where TIndex : class
            where TProjection : class
        {
            throw new NotImplementedException();
        }

        public IScannable<TProjection> Query<TIndex, TProjection>(IFilterable<TIndex> filter, Dictionary<string, Tuple<object, Type>> lastEvaluatedKey)
            where TIndex : class
            where TProjection : class
        {
            throw new NotImplementedException();
        }

        public IScannable<TModel> Scan(IFilterable<TModel> filter)
        {
            throw new NotImplementedException();
        }

        public IScannable<TModel> Scan(IFilterable<TModel> filter, Dictionary<string, Tuple<object, Type>> lastEvaluatedKey)
        {
            throw new NotImplementedException();
        }

        public IScannable<TModel> Scan()
        {
            throw new NotImplementedException();
        }

        public IScannable<TModel> Scan(Dictionary<string, Tuple<object, Type>> lastEvaluatedKey)
        {
            throw new NotImplementedException();
        }

        public Task Update(TModel item)
        {
            throw new NotImplementedException();
        }

        public Task Update(TModel item, IFilterable<TModel> condition)
        {
            throw new NotImplementedException();
        }

        public Task Update(dynamic item, IFilterable<TModel> condition)
        {
            throw new NotImplementedException();
        }

        private AmazonDynamoDBClient GetDynamoDbClient()
        {
            if (_credentials == null)
            {
                var config = new AmazonDynamoDBConfig();
                config.RegionEndpoint = _endpoint;
                return new AmazonDynamoDBClient(config);
            }
            
            return new AmazonDynamoDBClient(_credentials, _endpoint);
        }
        
    }
}


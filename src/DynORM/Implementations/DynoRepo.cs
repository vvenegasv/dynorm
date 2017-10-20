using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using DynORM.Helpers;
using DynORM.Interfaces;
using DynORM.Mappers;

namespace DynORM.Implementations
{
    public class DynoRepo<TModel> : IDynoRepo<TModel> where TModel : class
    {
        private readonly AWSCredentials _credentials;
        private readonly RegionEndpoint _endpoint;
        private readonly RequestMapper _requestMapper;

        internal DynoRepo(AWSCredentials credentials, RegionEndpoint endpoint)
        {
            _credentials = credentials;
            _endpoint = endpoint;
            _requestMapper = RequestMapper.Instance;
        }

        public bool IsConsistentRead { get; set; }

        public async Task Create(TModel item)
        {
            if(item == null)
                throw new ArgumentNullException(nameof(item));

            var client = GetDynamoDbClient();
            var putRequest = _requestMapper.ToRequest(item);
            var response = await client.PutItemAsync(putRequest);
        }

        public async Task Create(TModel item, IDynoFilter<TModel> condition)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            
            var client = GetDynamoDbClient();
            var putRequest = _requestMapper.ToRequest(item, condition);
            var response = await client.PutItemAsync(putRequest);
        }

        public Task Delete(TModel item)
        {
            throw new NotImplementedException();
        }

        public Task Delete(TModel item, IDynoFilter<TModel> condition)
        {
            throw new NotImplementedException();
        }

        public Task Delete(IDynoFilter<TModel> condition)
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

        public IDynoQuery<TModel> Query<THashKey>(THashKey hashKey) where THashKey : class
        {
            throw new NotImplementedException();
        }

        public IDynoQuery<TModel> Query<THashKey>(THashKey hashKey, Dictionary<string, Tuple<object, Type>> lastEvaluatedKey) where THashKey : class
        {
            throw new NotImplementedException();
        }

        public IDynoQuery<TProjection> Query<TIndex, TProjection>(IDynoFilter<TIndex> filter)
            where TIndex : class
            where TProjection : class
        {
            throw new NotImplementedException();
        }

        public IDynoQuery<TProjection> Query<TIndex, TProjection>(IDynoFilter<TIndex> filter, Dictionary<string, Tuple<object, Type>> lastEvaluatedKey)
            where TIndex : class
            where TProjection : class
        {
            throw new NotImplementedException();
        }

        public IDynoQuery<TModel> Scan(IDynoFilter<TModel> filter)
        {
            throw new NotImplementedException();
        }

        public IDynoQuery<TModel> Scan(IDynoFilter<TModel> filter, Dictionary<string, Tuple<object, Type>> lastEvaluatedKey)
        {
            throw new NotImplementedException();
        }

        public IDynoQuery<TModel> Scan()
        {
            throw new NotImplementedException();
        }

        public IDynoQuery<TModel> Scan(Dictionary<string, Tuple<object, Type>> lastEvaluatedKey)
        {
            throw new NotImplementedException();
        }

        public Task Update(TModel item)
        {
            throw new NotImplementedException();
        }

        public Task Update(TModel item, IDynoFilter<TModel> condition)
        {
            throw new NotImplementedException();
        }

        public Task Update(dynamic item, IDynoFilter<TModel> condition)
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


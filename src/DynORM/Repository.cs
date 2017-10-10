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

namespace DynORM
{
    public class Repository<TModel> : IRepository<TModel> where TModel : class
    {
        private readonly AWSCredentials _credentials;
        private readonly TableRequestBuilder<TModel> _tableRequestBuilder;
        private IList<Expression<Func<TModel, bool>>> _conditions;

        internal Repository(AWSCredentials credentials, RegionEndpoint endpoint, TableRequestBuilder<TModel> tableRequestBuilder)
        {
            _tableRequestBuilder = tableRequestBuilder;
            _credentials = credentials;
            _conditions = new List<Expression<Func<TModel, bool>>>();
        }

        public Repository<TModel> AddConditiion(Expression<Func<TModel, bool>> predicate)
        {
            _conditions.Add(predicate);
            return this;
        }

        public Task Create(TModel item)
        {
            foreach (Expression<Func<TModel, bool>> condition in _conditions)
            {
                var filter = _tableRequestBuilder.BuildExpression(condition);
            }

            
            using (var context = new DynamoDBContext(GetDynamoDbClient()))
            {
                return context.SaveAsync(item);
            }
        }

        public Task Delete(TModel item)
        {
            throw new NotImplementedException();
        }

        public Task Update(TModel item)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<TModel>> GetByHashKey<THashKey>(THashKey hashKey)
        {
            throw new NotImplementedException();
        }

        public Task<TModel> GetByPk<THashKey>(THashKey hashKey)
        {
            throw new NotImplementedException();
        }

        public Task<TModel> GetByPk<THashKey, TRangeKey>(THashKey hashKey, TRangeKey rangeKey)
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


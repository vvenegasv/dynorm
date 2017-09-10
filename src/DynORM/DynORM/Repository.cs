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

namespace DynORM
{
    public class Repository<TModel> : IRepository<TModel> where TModel : class
    {
        private readonly AmazonDynamoDBClient _dynamoClient;
        private readonly DynamoDBOperationConfig _dynamoDbOperationConfig;
        private IList<Expression<Func<TModel, bool>>> _conditions;

        public Repository(string enviromentPrefix, AmazonDynamoDBClient dynamoClient)
        {
            _dynamoClient = dynamoClient;
            
            //Get the table name from attibute or class name if this attribute does not exists
            var tableName = MetadataHelper.Instance.GetTableName<TModel>();

            //Make a new DynamoOperationConfig with the new table name
            _dynamoDbOperationConfig = new DynamoDBOperationConfig
            {
                OverrideTableName = $"{enviromentPrefix}{tableName}"
            };

            _conditions = new List<Expression<Func<TModel, bool>>>();
        }

        public Repository<TModel> AddConditiion(Expression<Func<TModel, bool>> predicate)
        {
            _conditions.Add(predicate);
            return this;
        }

        public Task Create(TModel item)
        {
            foreach (Expression<Func<TModel, bool>> c in _conditions)
            {
                var body = c.Body as BinaryExpression;
                var left = body.Left;
                var right = body.Right;
                var operationType = body.NodeType;


            }

            
            using (var context = new DynamoDBContext(_dynamoClient))
            {
                return context.SaveAsync(item, _dynamoDbOperationConfig);
            }
        }

        public Task Delete(TModel item)
        {
            throw new NotImplementedException();
        }

        public async Task Update(TModel item)
        {
            using (var context = new DynamoDBContext(_dynamoClient))
            {
                await context.SaveAsync(item, _dynamoDbOperationConfig);
            }
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

        
    }
}


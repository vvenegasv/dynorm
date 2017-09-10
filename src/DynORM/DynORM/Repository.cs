using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using DynORM.Helpers;

namespace DynORM
{
    public class Repository<TModel> : IRepository<TModel> where TModel : class
    {
        private readonly AmazonDynamoDBClient _dynamoClient;
        private readonly DynamoDBOperationConfig _dynamoDbOperationConfig;

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
        }

        public async Task Create(TModel item)
        {
            using (var context = new DynamoDBContext(_dynamoClient))
            {
                await context.SaveAsync(item, _dynamoDbOperationConfig);
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

        
    }
}

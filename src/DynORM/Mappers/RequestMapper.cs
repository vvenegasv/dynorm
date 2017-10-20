using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using DynORM.Helpers;
using DynORM.Interfaces;
using DynORM.Models;

namespace DynORM.Mappers
{
    internal class RequestMapper
    {
        private static volatile RequestMapper _instance;
        private static object _syncRoot = new Object();
        private readonly ItemMapper _itemMapper;
        private readonly ItemHelper _itemHelper;
       
        
        private RequestMapper()
        {
            _itemHelper = ItemHelper.Instance;
            _itemMapper = ItemMapper.Instance;
        }

        public static RequestMapper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                            _instance = new RequestMapper();
                    }
                }
                return _instance;
            }
        }

        public PutItemRequest ToRequest<TModel>(TModel item) where TModel : class
        {
            return new PutItemRequest
            {
                TableName = _itemHelper.GetTableName(item),
                Item = _itemMapper.ToItem(item)
            };
        }

        public PutItemRequest ToRequest<TModel>(TModel item, IDynoFilter<TModel> condition) where TModel : class
        {
            var builded = condition.Build();
            return new PutItemRequest
            {
                TableName = _itemHelper.GetTableName(item),
                Item = _itemMapper.ToItem(item),
                ConditionExpression = builded.GetQuery(),
                ExpressionAttributeValues = _itemMapper.ToValue(builded.GetValues()),
                ExpressionAttributeNames = builded.GetNames().ToDictionary(x => x.Key, x => x.Value)
            };
        }
    }
}


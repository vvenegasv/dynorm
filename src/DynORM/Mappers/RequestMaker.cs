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
    internal class RequestMaker<TItem> where TItem : class
    {
        private readonly ItemMapper _itemMapper;
        private readonly ItemHelper _itemHelper;
        private readonly TItem _item;
        
        
        public RequestMaker(TItem item)
        {
            _item = item;
            _itemHelper = ItemHelper.Instance;
            _itemMapper = ItemMapper.Instance;
        }

        public PutItemRequest ToRequest()
        {
            return new PutItemRequest
            {
                TableName = _itemHelper.GetTableName(_item),
                Item = _itemMapper.ToItem(_item)
            };
        }

        public PutItemRequest ToRequest(IDynoFilter<TItem> condition)
        {
            var builded = condition.Build();
            return new PutItemRequest
            {
                TableName = _itemHelper.GetTableName(_item),
                Item = _itemMapper.ToItem(_item),
                ConditionExpression = builded.GetQuery(),
                ExpressionAttributeValues = _itemMapper.ToValues(builded.GetValues()),
                ExpressionAttributeNames = builded.GetNames().ToDictionary(x => x.Key, x => x.Value)
            };
        }
    }
}


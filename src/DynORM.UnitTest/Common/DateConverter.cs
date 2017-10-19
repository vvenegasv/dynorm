using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using DynORM.Interfaces;

namespace DynORM.UnitTest.Common
{
    internal class DateConverter : IDynoConvert
    {
        public AttributeValue ToItem(object value)
        {
            var date = (DateTime) value;
            return new AttributeValue
            {
                N = date.Ticks.ToString(),
            };
        }

        public object ToValue(string item)
        {
            var ticks = Convert.ToInt64(item);
            return new DateTime(ticks);
        }
    }
}

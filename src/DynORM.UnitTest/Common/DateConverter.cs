using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using DynORM.Interfaces;

namespace DynORM.UnitTest.Common
{
    internal class DateConverter : IDynoConvert
    {
        public Tuple<string, Type> ToItem(object value)
        {
            var date = (DateTime)value;
            return new Tuple<string, Type>(date.Ticks.ToString(), typeof(DateTime));
        }

        public object ToValue(string item)
        {
            var ticks = Convert.ToInt64(item);
            return new DateTime(ticks);
        }
    }
}

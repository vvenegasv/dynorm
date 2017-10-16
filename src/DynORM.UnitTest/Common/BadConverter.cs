using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.DocumentModel;

namespace DynORM.UnitTest.Common
{
    internal class BadConverter
    {
        public object FromEntry(DynamoDBEntry entry)
        {
            return new DateTime(entry.AsLong());
        }

        public DynamoDBEntry ToEntry(object value)
        {
            var date = (DateTime)value;
            return new Primitive
            {
                Type = DynamoDBEntryType.Numeric,
                Value = date.Ticks
            };
        }
    }
}

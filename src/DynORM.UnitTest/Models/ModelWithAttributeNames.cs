using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.DataModel;
using DynORM.UnitTest.Common;

namespace DynORM.UnitTest.Models
{
    public class ModelWithAttributeNames
    {
        [DynamoDBHashKey("Another1", typeof(BadConverter))]
        public string Column1 { get; set; }

        [DynamoDBRangeKey("Another2")]
        public decimal Column2 { get; set; }

        [DynamoDBGlobalSecondaryIndexHashKey("first-index", AttributeName = "Another3", Converter = typeof(DateConverter))]
        public DateTime Column3 { get; set; }

        [DynamoDBGlobalSecondaryIndexRangeKey("first-index", AttributeName = "Another4")]
        public float Column4 { get; set; }

        [DynamoDBProperty("Another5")]
        public List<string> Column5 { get; set; }

        public bool Column6 { get; set; }
    }
}

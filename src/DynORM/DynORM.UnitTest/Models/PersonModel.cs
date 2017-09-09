using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.DataModel;

namespace DynORM.UnitTest.Models
{
    [DynamoDBTable("Test-PersonModel")]
    internal class PersonModel
    {
        [DynamoDBHashKey()]
        public string PersonId { get; set; }

        [DynamoDBProperty]
        public string Name { get; set; }

        [DynamoDBProperty]
        public string Phone { get; set; }

        [DynamoDBProperty]
        public string Email { get; set; }
    }
}

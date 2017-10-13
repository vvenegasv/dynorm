using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.DataModel;
using DynORM.UnitTest.Common;

namespace DynORM.UnitTest.Models
{
    [DynamoDBTable("Test-PersonModel")]
    internal class PersonModel
    {
        [DynamoDBHashKey(typeof(DateConverter))]
        public string PersonId { get; set; }
        
        [DynamoDBProperty]
        public string Name { get; set; }
        
        [DynamoDBProperty]
        public string Email { get; set; }

        [DynamoDBProperty]
        public List<PhoneModel> Phones { get; set; }
    }
}

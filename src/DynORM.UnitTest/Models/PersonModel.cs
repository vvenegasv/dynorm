﻿using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.DataModel;
using DynORM.Attributes;
using DynORM.Models;
using DynORM.UnitTest.Common;

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
        public string Email { get; set; }

        [PropertyType(PropertyType = PropertyType.String)]
        [DynamoDBProperty("UserAge")]
        public int Age { get; set; }

        [PropertyType(PropertyType = PropertyType.Number)]
        [DynamoDBProperty("created-at", Converter = typeof(DateConverter))]
        public DateTime CreatedAt { get; set; }

        [DynamoDBProperty]
        public List<PhoneModel> Phones { get; set; }
    }
}

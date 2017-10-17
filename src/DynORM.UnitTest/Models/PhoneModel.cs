using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.DataModel;

namespace DynORM.UnitTest.Models
{
    public class PhoneModel
    {
        public PhoneType PhoneType { get; set; }
        public string Number { get; set; }
    }
}

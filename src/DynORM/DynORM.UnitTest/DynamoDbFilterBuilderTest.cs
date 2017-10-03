using System;
using System.Collections.Generic;
using System.Text;
using DynORM.Filters;
using DynORM.UnitTest.Models;
using Xunit;

namespace DynORM.UnitTest
{
    public class DynamoDbFilterBuilderTest
    {
        [Fact]
        public void SimpleWhere()
        {
            IFilterBuilder<PersonModel> builder = new DynamoDbFilterBuilder<PersonModel>();
            var response = builder.Where(x => x.Name == "some name").Build();
            Assert.Equal("Name = some name", response);
        }
    }
}

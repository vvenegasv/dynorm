using System;
using System.Collections.Generic;
using System.Text;
using DynORM.UnitTest.Models;
using Xunit;

namespace DynORM.UnitTest
{
    public class ExpressionBuilderTest
    {
        [Fact]
        public void SimpleWhere()
        {
            var tableRequestBuilder = new TableRequestBuilder<PersonModel>("");
            var response = tableRequestBuilder.BuildExpression(x => x.Name == "some name");
            Assert.Equal("Name = some name", response);
        }

        [Fact]
        public void WhereWithTwoParts()
        {
            var tableRequestBuilder = new TableRequestBuilder<PersonModel>("");
            var response = tableRequestBuilder.BuildExpression(x => x.Name == "name" && x.Email != "dummy");
            Assert.Equal("Name = name AND Email <> dummy", response);
        }

        [Fact]
        public void WhereWithContains()
        {
            var tableRequestBuilder = new TableRequestBuilder<PersonModel>("");
            var names = new List<string>() {"n1", "n2", "n3"};
            var response = tableRequestBuilder.BuildExpression(x => names.Contains(x.Email));
            Assert.Equal("Name = name AND Email <> dummy", response);
        }
    }
}

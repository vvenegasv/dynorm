﻿using System;
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
        public void SingleExpression()
        {
            IFilterBuilder<PersonModel> builder = new DynamoDbFilterBuilder<PersonModel>();
            var compiledFilter = builder.Where(x => x.Name == "some name").Build();
            var query = compiledFilter.GetQuery();
            var values = compiledFilter.GetValues();
            var names = compiledFilter.GetNames();

            Assert.Equal("#Name = :p1", query);
            Assert.Equal(values?.ContainsKey(":p1"), true);
            Assert.Equal(values?[":p1"].Key, "some name");
            Assert.Equal(names?.ContainsKey("#Name"), true);
            Assert.Equal(names?["#Name"], "Name");
        }

        [Fact]
        public void TwoExpression()
        {
            IFilterBuilder<PersonModel> builder = new DynamoDbFilterBuilder<PersonModel>();
            var compiledFilter = builder
                .Where(x => x.Email == "n1" && x.Name == "n2")
                .Build();
            var query = compiledFilter.GetQuery();
            var values = compiledFilter.GetValues();
            var names = compiledFilter.GetNames();

            Assert.Equal("#Email = :p1 AND #Name = :p2", query);
            Assert.Equal(values?.ContainsKey(":p1"), true);
            Assert.Equal(values?.ContainsKey(":p2"), true);
            Assert.Equal(values?[":p1"].Key, "n1");
            Assert.Equal(values?[":p2"].Key, "n2");
            Assert.Equal(names?.ContainsKey("#Email"), true);
            Assert.Equal(names?.ContainsKey("#Name"), true);
            Assert.Equal(names?["#Name"], "Name");
            Assert.Equal(names?["#Email"], "Email");
        }

        [Fact]
        public void TwoWhere()
        {
            IFilterBuilder<PersonModel> builder = new DynamoDbFilterBuilder<PersonModel>();
            var compiledFilter = builder
                .Where(x => x.Email == "n1" && x.Name == "n2")
                .Where(x => x.Phone == "123", FilterConcatenationType.Or)
                .Build();
            var query = compiledFilter.GetQuery();
            var values = compiledFilter.GetValues();
            var names = compiledFilter.GetNames();

            Assert.Equal("#Email = :p1 AND #Name = :p2 OR #Phone = :p3", query);
            Assert.Equal(values?.ContainsKey(":p1"), true);
            Assert.Equal(values?.ContainsKey(":p2"), true);
            Assert.Equal(values?.ContainsKey(":p3"), true);
            Assert.Equal(values?[":p1"].Key, "n1");
            Assert.Equal(values?[":p2"].Key, "n2");
            Assert.Equal(values?[":p3"].Key, "123");
            Assert.Equal(names?.ContainsKey("#Email"), true);
            Assert.Equal(names?.ContainsKey("#Name"), true);
            Assert.Equal(names?.ContainsKey("#Phone"), true);
            Assert.Equal(names?["#Name"], "Name");
            Assert.Equal(names?["#Email"], "Email");
            Assert.Equal(names?["#Phone"], "Phone");
        }

        [Fact]
        public void FilterAsParameter()
        {
            IFilterBuilder<PersonModel> filter = new DynamoDbFilterBuilder<PersonModel>();
            filter
                .Where(x => x.Email == "n1" && x.Name == "n2")
                .Where(x => x.Phone == "123", FilterConcatenationType.Or);

            var compiledFilter = new DynamoDbFilterBuilder<PersonModel>()
                .Where(filter)
                .Build();

            var query = compiledFilter.GetQuery();
            Assert.Equal("(#Email = :p1 AND #Name = :p2 OR #Phone = :p3)", query);
        }

        [Fact]
        public void In()
        {
            IFilterBuilder<PersonModel> builder = new DynamoDbFilterBuilder<PersonModel>();
            var compiledFilter = builder
                .WhereIn(x => x.Email, new List<string>() { "n1", "n2", "n3" })
                .Build();

            var query = compiledFilter.GetQuery();
            var values = compiledFilter.GetValues();
            var names = compiledFilter.GetNames();

            Assert.Equal("#Email in (:p1, :p2, :p3)", query);
            Assert.Equal(values?.ContainsKey(":p1"), true);
            Assert.Equal(values?[":p1"].Key, "n1");
            Assert.Equal(names?.ContainsKey("#Email"), true);
            Assert.Equal(names?["#Email"], "Email");
        }

        [Fact]
        public void AttributeExists()
        {
            IFilterBuilder<PersonModel> builder = new DynamoDbFilterBuilder<PersonModel>();
            var compiledFilter = builder
                .WhereAttributeExists(x => x.Email)
                .Build();

            var query = compiledFilter.GetQuery();
            var values = compiledFilter.GetValues();
            var names = compiledFilter.GetNames();
            
            Assert.Equal("attribute_exists (#Email)", query);
            Assert.Equal(values.Count, 0);
            Assert.Equal(names?.ContainsKey("#Email"), true);
            Assert.Equal(names?["#Email"], "Email");
        }

        [Fact]
        public void AttributeExistsAsString()
        {
            IFilterBuilder<PersonModel> builder = new DynamoDbFilterBuilder<PersonModel>();
            var compiledFilter = builder
                .WhereAttributeExists("Correo")
                .Build();

            var query = compiledFilter.GetQuery();
            Assert.Equal("attribute_exists (#Correo)", query);
        }

        [Fact]
        public void InvalidWhere()
        {
            var correctError = false;
            IFilterBuilder<PersonModel> builder = new DynamoDbFilterBuilder<PersonModel>();
            try
            {
                var query = builder.Where(x => x.Name.EndsWith("invalid!")).Build();
            }
            catch(ExpressionNotSupportedException ex)
            {
                correctError = true;
            }
            catch (Exception ex)
            {
                correctError = false;
            }

            Assert.Equal(correctError, true);
        }
    }
}

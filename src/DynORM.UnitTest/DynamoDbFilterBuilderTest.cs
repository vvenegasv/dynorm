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
        public void SingleExpression()
        {
            IFilterBuilder<PersonModel> builder = new DynamoDbFilterBuilder<PersonModel>();
            var response = builder.Where(x => x.Name == "some name").Build();
            Assert.Equal("#Name = :p1", response);
        }

        [Fact]
        public void TwoExpression()
        {
            IFilterBuilder<PersonModel> builder = new DynamoDbFilterBuilder<PersonModel>();
            var response = builder
                .Where(x => x.Email == "n1" && x.Name == "n2")
                .Build();
            Assert.Equal("#Email = :p1 AND #Name = p2", response);
        }

        [Fact]
        public void TwoWhere()
        {
            IFilterBuilder<PersonModel> builder = new DynamoDbFilterBuilder<PersonModel>();
            var response = builder
                .Where(x => x.Email == "n1" && x.Name == "n2")
                .Where(x => x.Phone == "123", FilterConcatenationType.Or)
                .Build();
            Assert.Equal("#Email = :p1 AND #Name = :p2 OR #Phone = :p3", response);
        }

        [Fact]
        public void FilterAsParameter()
        {
            IFilterBuilder<PersonModel> filter = new DynamoDbFilterBuilder<PersonModel>();
            filter
                .Where(x => x.Email == "n1" && x.Name == "n2")
                .Where(x => x.Phone == "123", FilterConcatenationType.Or);

            var response = new DynamoDbFilterBuilder<PersonModel>()
                .Where(filter)
                .Build();

            Assert.Equal("(#Email = :p1 AND #Name = :p2 OR #Phone = :p3)", response);
        }

        [Fact]
        public void In()
        {
            IFilterBuilder<PersonModel> builder = new DynamoDbFilterBuilder<PersonModel>();
            var response = builder
                .WhereIn(x => x.Email, new List<string>() { "n1", "n2", "n3" })
                .Build();
            Assert.Equal("#Email in (:p1, :p2, :p3)", response);
        }

        [Fact]
        public void AttributeExists()
        {
            IFilterBuilder<PersonModel> builder = new DynamoDbFilterBuilder<PersonModel>();
            var response = builder
                .WhereAttributeExists(x => x.Email)
                .Build();
            Assert.Equal("attribute_exists (#Email)", response);
        }

        [Fact]
        public void AttributeExistsAsString()
        {
            IFilterBuilder<PersonModel> builder = new DynamoDbFilterBuilder<PersonModel>();
            var response = builder
                .WhereAttributeExists("Correo")
                .Build();
            Assert.Equal("attribute_exists (#Correo)", response);
        }

        [Fact]
        public void InvalidWhere()
        {
            var correctError = false;
            IFilterBuilder<PersonModel> builder = new DynamoDbFilterBuilder<PersonModel>();
            try
            {
                var response = builder.Where(x => x.Name.EndsWith("invalid!")).Build();
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

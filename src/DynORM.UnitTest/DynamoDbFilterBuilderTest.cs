using System;
using System.Collections.Generic;
using System.Text;
using DynORM.Enums;
using DynORM.Exceptions;
using DynORM.Implementations;
using DynORM.Interfaces;
using DynORM.Models;
using DynORM.UnitTest.Common;
using DynORM.UnitTest.Models;
using Xunit;

namespace DynORM.UnitTest
{
    public class DynamoDbFilterBuilderTest
    {
        [Fact]
        public void SingleExpression()
        {
            IDynoFilter<PersonModel> builder = new DynoFilterBuilder<PersonModel>();
            var compiledFilter = builder.Where(x => x.Name == "some name").Build();
            var query = compiledFilter.GetQuery();
            var values = compiledFilter.GetValues();
            var names = compiledFilter.GetNames();

            Assert.Equal("#Name = :p1", query);
            Assert.Equal(values?.ContainsKey(":p1"), true);
            Assert.Equal(values?[":p1"].Item1, "some name");
            Assert.Equal(values?[":p1"].Item2, PropertyType.String);
            Assert.Equal(values?[":p1"].Item3, null);
            Assert.Equal(names?.ContainsKey("#Name"), true);
            Assert.Equal(names?["#Name"], "Name");
        }

        [Fact]
        public void TwoExpression()
        {
            IDynoFilter<PersonModel> builder = new DynoFilterBuilder<PersonModel>();
            var compiledFilter = builder
                .Where(x => x.Email == "n1" && x.Name == "n2")
                .Build();
            var query = compiledFilter.GetQuery();
            var values = compiledFilter.GetValues();
            var names = compiledFilter.GetNames();

            Assert.Equal("#Email = :p1 AND #Name = :p2", query);
            Assert.Equal(values?.ContainsKey(":p1"), true);
            Assert.Equal(values?.ContainsKey(":p2"), true);
            Assert.Equal(values?[":p1"].Item1, "n1");
            Assert.Equal(values?[":p1"].Item2, PropertyType.String);
            Assert.Equal(values?[":p1"].Item3, null);
            Assert.Equal(values?[":p2"].Item1, "n2");
            Assert.Equal(values?[":p2"].Item2, PropertyType.String);
            Assert.Equal(values?[":p2"].Item3, null);
            Assert.Equal(names?.ContainsKey("#Email"), true);
            Assert.Equal(names?.ContainsKey("#Name"), true);
            Assert.Equal(names?["#Name"], "Name");
            Assert.Equal(names?["#Email"], "Email");
        }

        [Fact]
        public void TwoWhere()
        {
            IDynoFilter<PersonModel> builder = new DynoFilterBuilder<PersonModel>();
            var compiledFilter = builder
                .Where(x => x.Email == "n1" && x.Name == "n2")
                .Where(x => x.Age < 40, FilterConcatenationType.Or)
                .Build();
            var query = compiledFilter.GetQuery();
            var values = compiledFilter.GetValues();
            var names = compiledFilter.GetNames();

            Assert.Equal("#Email = :p1 AND #Name = :p2 OR #UserAge < :p3", query);
            Assert.Equal(values?.ContainsKey(":p1"), true);
            Assert.Equal(values?.ContainsKey(":p2"), true);
            Assert.Equal(values?.ContainsKey(":p3"), true);
            Assert.Equal(values?[":p1"].Item1, "n1");
            Assert.Equal(values?[":p1"].Item2, PropertyType.String);
            Assert.Equal(values?[":p1"].Item3, null);
            Assert.Equal(values?[":p2"].Item1, "n2");
            Assert.Equal(values?[":p2"].Item2, PropertyType.String);
            Assert.Equal(values?[":p2"].Item3, null);
            Assert.Equal(values?[":p3"].Item1, 40);
            Assert.Equal(values?[":p3"].Item2, PropertyType.String);
            Assert.Equal(values?[":p3"].Item3, null);
            Assert.Equal(names?.ContainsKey("#Email"), true);
            Assert.Equal(names?.ContainsKey("#Name"), true);
            Assert.Equal(names?.ContainsKey("#UserAge"), true);
            Assert.Equal(names?["#Name"], "Name");
            Assert.Equal(names?["#Email"], "Email");
            Assert.Equal(names?["#UserAge"], "UserAge");
        }

        [Fact]
        public void BigWhere()
        {
            IDynoFilter<PersonModel> builder = new DynoFilterBuilder<PersonModel>();
            var compiledFilter = builder
                .Where(x => x.Email == "n1" && (x.Name == "n2" || x.Age < 40))
                .Build();
            var query = compiledFilter.GetQuery();
            var values = compiledFilter.GetValues();
            var names = compiledFilter.GetNames();

            Assert.Equal("#Email = :p1 AND (#Name = :p2 OR #UserAge < :p3)", query);
            Assert.Equal(values?.ContainsKey(":p1"), true);
            Assert.Equal(values?.ContainsKey(":p2"), true);
            Assert.Equal(values?.ContainsKey(":p3"), true);
            Assert.Equal(values?[":p1"].Item1, "n1");
            Assert.Equal(values?[":p1"].Item2, PropertyType.String);
            Assert.Equal(values?[":p1"].Item3, null);
            Assert.Equal(values?[":p2"].Item1, "n2");
            Assert.Equal(values?[":p2"].Item2, PropertyType.String);
            Assert.Equal(values?[":p2"].Item3, null);
            Assert.Equal(values?[":p3"].Item1, 40);
            Assert.Equal(values?[":p3"].Item2, PropertyType.String);
            Assert.Equal(values?[":p3"].Item3, null);
            Assert.Equal(names?.ContainsKey("#Email"), true);
            Assert.Equal(names?.ContainsKey("#Name"), true);
            Assert.Equal(names?.ContainsKey("#UserAge"), true);
            Assert.Equal(names?["#Name"], "Name");
            Assert.Equal(names?["#Email"], "Email");
            Assert.Equal(names?["#UserAge"], "UserAge");
        }

        [Fact]
        public void BigWhereWithTwoParentesis()
        {
            IDynoFilter<PersonModel> builder = new DynoFilterBuilder<PersonModel>();
            var compiledFilter = builder
                .Where(x => (x.Email == "n1" && x.Name == "n2") || (x.Age < 40 && x.PersonId == "123"))
                .Build();
            var query = compiledFilter.GetQuery();
            var values = compiledFilter.GetValues();
            var names = compiledFilter.GetNames();

            Assert.Equal("(#Email = :p1 AND #Name = :p2) OR (#UserAge < :p3 AND #PersonId = :p4)", query);
            Assert.Equal(values?.ContainsKey(":p1"), true);
            Assert.Equal(values?.ContainsKey(":p2"), true);
            Assert.Equal(values?.ContainsKey(":p3"), true);
            Assert.Equal(values?.ContainsKey(":p4"), true);
            Assert.Equal(values?[":p1"].Item1, "n1");
            Assert.Equal(values?[":p1"].Item2, PropertyType.String);
            Assert.Equal(values?[":p1"].Item3, null);
            Assert.Equal(values?[":p2"].Item1, "n2");
            Assert.Equal(values?[":p2"].Item2, PropertyType.String);
            Assert.Equal(values?[":p2"].Item3, null);
            Assert.Equal(values?[":p3"].Item1, 40);
            Assert.Equal(values?[":p3"].Item2, PropertyType.String);
            Assert.Equal(values?[":p3"].Item3, null);
            Assert.Equal(values?[":p4"].Item1, "123");
            Assert.Equal(values?[":p4"].Item2, PropertyType.String);
            Assert.Equal(values?[":p4"].Item3, null);
            Assert.Equal(names?.ContainsKey("#Email"), true);
            Assert.Equal(names?.ContainsKey("#Name"), true);
            Assert.Equal(names?.ContainsKey("#UserAge"), true);
            Assert.Equal(names?["#Name"], "Name");
            Assert.Equal(names?["#Email"], "Email");
            Assert.Equal(names?["#UserAge"], "UserAge");
        }

        [Fact]
        public void FilterAsParameter()
        {
            var date = new DateTime(2017, 1, 1);
            IDynoFilter<PersonModel> filter = new DynoFilterBuilder<PersonModel>();
            filter
                .Where(x => x.CreatedAt <= date && x.Name == "n2")
                .Where(x => x.PersonId == "123", FilterConcatenationType.Or);

            var compiledFilter = new DynoFilterBuilder<PersonModel>()
                .Where(filter)
                .Build();

            var query = compiledFilter.GetQuery();
            var names = compiledFilter.GetNames();
            var values = compiledFilter.GetValues();

            Assert.Equal("(#created-at <= :p1 AND #Name = :p2 OR #PersonId = :p3)", query);
            Assert.Equal(names?.ContainsKey("#created-at"), true);
            Assert.Equal(names?.ContainsKey("#Name"), true);
            Assert.Equal(names?.ContainsKey("#PersonId"), true);
            Assert.Equal(names?["#PersonId"], "PersonId");
            Assert.Equal(names?["#Name"], "Name");
            Assert.Equal(names?["#created-at"], "created-at");
            Assert.Equal(values?.ContainsKey(":p1"), true);
            Assert.Equal(values?.ContainsKey(":p2"), true);
            Assert.Equal(values?.ContainsKey(":p3"), true);
            Assert.Equal(values?[":p1"].Item1, date);
            Assert.Equal(values?[":p1"].Item2, PropertyType.Number);
            Assert.Equal(values?[":p1"].Item3, typeof(DateConverter));
            Assert.Equal(values?[":p2"].Item1, "n2");
            Assert.Equal(values?[":p2"].Item2, PropertyType.String);
            Assert.Equal(values?[":p2"].Item3, null);
            Assert.Equal(values?[":p3"].Item1, "123");
            Assert.Equal(values?[":p3"].Item2, PropertyType.String);
            Assert.Equal(values?[":p3"].Item3, null);
        }

        [Fact]
        public void In()
        {
            IDynoFilter<PersonModel> builder = new DynoFilterBuilder<PersonModel>();
            var compiledFilter = builder
                .WhereIn(x => x.Email, new List<string>() { "n1", "n2", "n3" })
                .Build();

            var query = compiledFilter.GetQuery();
            var values = compiledFilter.GetValues();
            var names = compiledFilter.GetNames();

            Assert.Equal("#Email in (:p1, :p2, :p3)", query);
            Assert.Equal(values?.ContainsKey(":p1"), true);
            Assert.Equal(values?.ContainsKey(":p2"), true);
            Assert.Equal(values?.ContainsKey(":p3"), true);
            Assert.Equal(values?[":p1"].Item1, "n1");
            Assert.Equal(values?[":p1"].Item2, PropertyType.String);
            Assert.Equal(values?[":p1"].Item3, null);
            Assert.Equal(values?[":p2"].Item1, "n2");
            Assert.Equal(values?[":p2"].Item2, PropertyType.String);
            Assert.Equal(values?[":p2"].Item3, null);
            Assert.Equal(values?[":p3"].Item1, "n3");
            Assert.Equal(values?[":p3"].Item2, PropertyType.String);
            Assert.Equal(values?[":p3"].Item3, null);
            Assert.Equal(names?.ContainsKey("#Email"), true);
            Assert.Equal(names?["#Email"], "Email");
        }

        [Fact]
        public void InWithConverters()
        {
            IDynoFilter<PersonModel> builder = new DynoFilterBuilder<PersonModel>();
            var random = new Random();
            var dt1 = new DateTime(random.Next(2000, DateTime.Now.Year), random.Next(1, 12), random.Next(1, 28));
            var dt2 = new DateTime(random.Next(2000, DateTime.Now.Year), random.Next(1, 12), random.Next(1, 28));
            var dt3 = new DateTime(random.Next(2000, DateTime.Now.Year), random.Next(1, 12), random.Next(1, 28));

            var compiledFilter = builder
                .WhereIn(x => x.CreatedAt, new List<DateTime>() { dt1, dt2, dt3 })
                .Build();

            var query = compiledFilter.GetQuery();
            var values = compiledFilter.GetValues();
            var names = compiledFilter.GetNames();

            Assert.Equal("#created-at in (:p1, :p2, :p3)", query);
            Assert.Equal(values?.ContainsKey(":p1"), true);
            Assert.Equal(values?.ContainsKey(":p2"), true);
            Assert.Equal(values?.ContainsKey(":p3"), true);
            Assert.Equal(values?[":p1"].Item1, dt1);
            Assert.Equal(values?[":p1"].Item2, PropertyType.Number);
            Assert.Equal(values?[":p1"].Item3, typeof(DateConverter));
            Assert.Equal(values?[":p2"].Item1, dt2);
            Assert.Equal(values?[":p2"].Item2, PropertyType.Number);
            Assert.Equal(values?[":p2"].Item3, typeof(DateConverter));
            Assert.Equal(values?[":p3"].Item1, dt3);
            Assert.Equal(values?[":p3"].Item2, PropertyType.Number);
            Assert.Equal(values?[":p3"].Item3, typeof(DateConverter));
            Assert.Equal(names?.ContainsKey("#created-at"), true);
            Assert.Equal(names?["#created-at"], "created-at");
        }

        [Fact]
        public void AttributeExists()
        {
            IDynoFilter<PersonModel> builder = new DynoFilterBuilder<PersonModel>();
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
            IDynoFilter<PersonModel> builder = new DynoFilterBuilder<PersonModel>();
            var compiledFilter = builder
                .WhereAttributeExists("Correo")
                .Build();

            var query = compiledFilter.GetQuery();
            var names = compiledFilter.GetNames();
            var values = compiledFilter.GetValues();

            Assert.Equal("attribute_exists (#Correo)", query);
            Assert.Equal(values.Count, 0);
            Assert.Equal(names?.ContainsKey("#Correo"), true);
            Assert.Equal(names?["#Correo"], "Correo");
        }

        [Fact]
        public void AttributeNotExists()
        {
            IDynoFilter<PersonModel> builder = new DynoFilterBuilder<PersonModel>();
            var compiledFilter = builder
                .WhereAttributeNotExists(x => x.Email)
                .Build();

            var query = compiledFilter.GetQuery();
            var values = compiledFilter.GetValues();
            var names = compiledFilter.GetNames();

            Assert.Equal("attribute_not_exists (#Email)", query);
            Assert.Equal(values.Count, 0);
            Assert.Equal(names?.ContainsKey("#Email"), true);
            Assert.Equal(names?["#Email"], "Email");
        }

        [Fact]
        public void AttributeNotExistsAsString()
        {
            IDynoFilter<PersonModel> builder = new DynoFilterBuilder<PersonModel>();
            var compiledFilter = builder
                .WhereAttributeNotExists("Correo")
                .Build();

            var query = compiledFilter.GetQuery();
            var values = compiledFilter.GetValues();
            var names = compiledFilter.GetNames();

            Assert.Equal("attribute_not_exists (#Correo)", query);
            Assert.Equal(values.Count, 0);
            Assert.Equal(names?.ContainsKey("#Correo"), true);
            Assert.Equal(names?["#Correo"], "Correo");
        }

        [Fact]
        public void BeginsWith()
        {
            IDynoFilter<PersonModel> builder = new DynoFilterBuilder<PersonModel>();
            var compiledFilter = builder
                .WhereBeginsWith(x => x.Age, "10")
                .Build();

            var query = compiledFilter.GetQuery();
            var values = compiledFilter.GetValues();
            var names = compiledFilter.GetNames();

            Assert.Equal("begins_with (#UserAge, :p1)", query);
            Assert.Equal(values?.Count, 1);
            Assert.Equal(names?.Count, 1);
            Assert.Equal(names?.ContainsKey("#UserAge"), true);
            Assert.Equal(names?["#UserAge"], "UserAge");
            Assert.Equal(values?.ContainsKey(":p1"), true);
            Assert.Equal(values?[":p1"].Item1, "10");
            Assert.Equal(values?[":p1"].Item2, PropertyType.String);
            Assert.Equal(values?[":p1"].Item3, null);
        }

        [Fact]
        public void Contains()
        {
            IDynoFilter<PersonModel> builder = new DynoFilterBuilder<PersonModel>();
            var compiledFilter = builder
                .WhereContains(x => x.Email, "n1")
                .Build();

            var query = compiledFilter.GetQuery();
            var values = compiledFilter.GetValues();
            var names = compiledFilter.GetNames();

            Assert.Equal("contains (#Email, :p1)", query);
            Assert.Equal(values?.Count, 1);
            Assert.Equal(names?.Count, 1);
            Assert.Equal(names?.ContainsKey("#Email"), true);
            Assert.Equal(names?["#Email"], "Email");
            Assert.Equal(values?.ContainsKey(":p1"), true);
            Assert.Equal(values?[":p1"].Item1, "n1");
            Assert.Equal(values?[":p1"].Item2, PropertyType.String);
            Assert.Equal(values?[":p1"].Item3, null);
        }

        [Fact]
        public void ContainsPhone()
        {
            var phone = new PhoneModel {PhoneType = PhoneType.CellPhone, Number = "954865498"};

            IDynoFilter<PersonModel> builder = new DynoFilterBuilder<PersonModel>();
            var compiledFilter = builder
                .WhereContains(x => x.Phones, phone)
                .Build();

            var query = compiledFilter.GetQuery();
            var values = compiledFilter.GetValues();
            var names = compiledFilter.GetNames();

            Assert.Equal("contains (#Phones, :p1)", query);
            Assert.Equal(values?.Count, 1);
            Assert.Equal(names?.Count, 1);
            Assert.Equal(names?.ContainsKey("#Phones"), true);
            Assert.Equal(names?["#Phones"], "Phones");
            Assert.Equal(values?.ContainsKey(":p1"), true);
            Assert.Equal(values?[":p1"].Item1, phone);
            Assert.Equal(values?[":p1"].Item2, PropertyType.String);
            Assert.Equal(values?[":p1"].Item3, null);
        }

        [Fact]
        public void Size()
        {
            IDynoFilter<PersonModel> builder = new DynoFilterBuilder<PersonModel>();
            var compiledFilter = builder
                .WhereSize(x => x.Email, ComparisonType.Greater, 20)
                .Build();

            var query = compiledFilter.GetQuery();
            var values = compiledFilter.GetValues();
            var names = compiledFilter.GetNames();

            Assert.Equal("size (#Email) > :p1)", query);
            Assert.Equal(values?.Count, 1);
            Assert.Equal(names?.Count, 1);
            Assert.Equal(names?.ContainsKey("#Email"), true);
            Assert.Equal(names?["#Email"], "Email");
            Assert.Equal(values?.ContainsKey(":p1"), true);
            Assert.Equal(values?[":p1"].Item1, 20);
            Assert.Equal(values?[":p1"].Item2, PropertyType.Number);
            Assert.Equal(values?[":p1"].Item3, null);
        }

        [Fact]
        public void InvalidWhere()
        {
            IDynoFilter<PersonModel> builder = new DynoFilterBuilder<PersonModel>();
            try
            {
                var query = builder.Where(x => x.Name.EndsWith("invalid!")).Build();
                throw new Exception("ExpressionNotSupportedException was not raised!");
            }
            catch (Exception ex)
            {
                Assert.IsType<ExpressionNotSupportedException>(ex);
            }
        }
    }
}

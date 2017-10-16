using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DocumentModel;
using DynORM.Exceptions;
using DynORM.Helpers;
using DynORM.Mappers;
using DynORM.Models;
using DynORM.UnitTest.Common;
using DynORM.UnitTest.Models;
using Xunit;

namespace DynORM.UnitTest
{
    public class ItemHelperTest
    {
        
        private readonly ItemHelper _itemHelper;

        public ItemHelperTest()
        {
            _itemHelper = ItemHelper.Instance;
        }
        
        [Fact]
        public void HashKeyColumnName()
        {
            var memberExpression = GetLeftMemberInfo<ModelWithAttributeNames>(x => x.Column1 == "asd");
            var name = _itemHelper.GetColumnName(memberExpression);
            Assert.Equal(name, "Another1");
        }

        [Fact]
        public void RangeKeyColumnName()
        {
            var memberExpression = GetLeftMemberInfo<ModelWithAttributeNames>(x => x.Column2 > 10);
            var name = _itemHelper.GetColumnName(memberExpression);
            Assert.Equal(name, "Another2");
        }

        [Fact]
        public void GsiHashKeyColumnName()
        {
            var memberExpression = GetLeftMemberInfo<ModelWithAttributeNames>(x => x.Column3 <= DateTime.Now);
            var name = _itemHelper.GetColumnName(memberExpression);
            Assert.Equal(name, "Another3");
        }

        [Fact]
        public void GsiRangeKeyColumnName()
        {
            var memberExpression = GetLeftMemberInfo<ModelWithAttributeNames>(x => x.Column4 > 10.4f);
            var name = _itemHelper.GetColumnName(memberExpression);
            Assert.Equal(name, "Another4");
        }

        [Fact]
        public void PropertyColumnName()
        {
            var memberExpression = GetLeftMemberInfo<ModelWithAttributeNames>(x => x.Column5 == new List<string>());
            var name = _itemHelper.GetColumnName(memberExpression);
            Assert.Equal(name, "Another5");
        }

        [Fact]
        public void DefaultColumnName()
        {
            var memberExpression = GetLeftMemberInfo<ModelWithAttributeNames>(x => x.Column6 == false);
            var name = _itemHelper.GetColumnName(memberExpression);
            Assert.Equal(name, "Column6");
        }

        


        [Fact]
        public void HashKeyColumnType()
        {
            var memberExpression = GetLeftMemberInfo<ModelWithAttributeNames>(x => x.Column1 == "asd");
            var columnType = _itemHelper.GetColumnType(memberExpression);
            Assert.Equal(columnType, PropertyType.String);
        }

        [Fact]
        public void RangeKeyColumnType()
        {
            var memberExpression = GetLeftMemberInfo<ModelWithAttributeNames>(x => x.Column2 > 10);
            var columnType = _itemHelper.GetColumnType(memberExpression);
            Assert.Equal(columnType, PropertyType.Number);
        }

        [Fact]
        public void GsiHashKeyColumnType()
        {
            var memberExpression = GetLeftMemberInfo<ModelWithAttributeNames>(x => x.Column3 <= DateTime.Now);
            var columnType = _itemHelper.GetColumnType(memberExpression);
            Assert.Equal(columnType, PropertyType.String);
        }

        [Fact]
        public void GsiRangeKeyColumnType()
        {
            var memberExpression = GetLeftMemberInfo<ModelWithAttributeNames>(x => x.Column4 > 10.4f);
            var columnType = _itemHelper.GetColumnType(memberExpression);
            Assert.Equal(columnType, PropertyType.Number);
        }

        [Fact]
        public void PropertyColumnType()
        {
            var memberExpression = GetLeftMemberInfo<ModelWithAttributeNames>(x => x.Column5 == new List<string>());
            var columnType = _itemHelper.GetColumnType(memberExpression);
            Assert.Equal(columnType, PropertyType.String);
        }

        [Fact]
        public void DefaultColumnType()
        {
            var memberExpression = GetLeftMemberInfo<ModelWithAttributeNames>(x => x.Column6 == false);
            var columnType = _itemHelper.GetColumnType(memberExpression);
            Assert.Equal(columnType, PropertyType.Boolean);
        }




        [Fact]
        public void HashKeyColumnConverter()
        {
            var memberExpression = GetLeftMemberInfo<ModelWithAttributeNames>(x => x.Column1 == "asd");
            try
            {
                var converter = _itemHelper.GetColumnConverter(memberExpression);
                throw new Exception("Exception was not raised. It's bad, because converter is not valid");
            }
            catch (Exception ex)
            {
                Assert.IsType<InvalidConverter>(ex);
            }
        }

        [Fact]
        public void RangeKeyColumnConverter()
        {
            var memberExpression = GetLeftMemberInfo<ModelWithAttributeNames>(x => x.Column2 > 10);
            var converter = _itemHelper.GetColumnConverter(memberExpression);
            Assert.Equal(converter, null);
        }

        [Fact]
        public void GsiHashKeyColumnConverter()
        {
            var memberExpression = GetLeftMemberInfo<ModelWithAttributeNames>(x => x.Column3 <= DateTime.Now);
            var converter = _itemHelper.GetColumnConverter(memberExpression);
            Assert.Equal(converter, typeof(DateConverter));
        }

        [Fact]
        public void GsiRangeKeyColumnConverter()
        {
            var memberExpression = GetLeftMemberInfo<ModelWithAttributeNames>(x => x.Column4 > 10.4f);
            var converter = _itemHelper.GetColumnConverter(memberExpression);
            Assert.Equal(converter, null);
        }

        [Fact]
        public void PropertyColumnConverter()
        {
            var memberExpression = GetLeftMemberInfo<ModelWithAttributeNames>(x => x.Column5 == new List<string>());
            var converter = _itemHelper.GetColumnConverter(memberExpression);
            Assert.Equal(converter, null);
        }

        [Fact]
        public void DefaultColumnConverter()
        {
            var memberExpression = GetLeftMemberInfo<ModelWithAttributeNames>(x => x.Column6 == false);
            var converter = _itemHelper.GetColumnConverter(memberExpression);
            Assert.Equal(converter, null);
        }

        [Fact]
        public void CustomTableName()
        {
            var tableName = _itemHelper.GetTableName(new ModelWithAttributeNames());
            Assert.Equal(tableName, "ModelWithAttributeNames");
        }

        [Fact]
        public void DefaultTableName()
        {
            var tableName = _itemHelper.GetTableName(new PersonModel());
            Assert.Equal(tableName, "Test-PersonModel");
        }

        private MemberExpression GetLeftMemberInfo<TModel>(Expression<Func<TModel, bool>> expression)
        {
            var binaryExpression = expression.Body as BinaryExpression;
            return binaryExpression.Left as MemberExpression;
        }
    }
}

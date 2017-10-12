using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DynORM.Helpers;
using DynORM.UnitTest.Common;
using DynORM.UnitTest.Models;
using Xunit;

namespace DynORM.UnitTest
{
    public class MetadataHelperTest
    {
        private readonly MetadataHelper _helper;

        public MetadataHelperTest()
        {
            _helper = MetadataHelper.Instance;
        }

        [Fact]
        public void IsPrimitiveStringList()
        {
            var data = new List<string>();
            data.Add("asd1");
            data.Add("asd2");
            data.Add("asd3");
            var type = data.GetType();
            var result = _helper.IsList(type) && _helper.GenericArgumentIsString(type);

            Assert.True(result);
        }

        [Fact]
        public void IsPrimitiveIntList()
        {
            var data = new List<int>();
            data.Add(1);
            data.Add(2);
            data.Add(3);
            var type = data.GetType();
            var result = _helper.IsList(type) && _helper.GenericArgumentIsNumber(type);

            Assert.True(result);
        }

        [Fact]
        public void IsPrimitiveLongList()
        {
            var data = new List<long>();
            data.Add(1);
            data.Add(2);
            data.Add(3);
            var type = data.GetType();
            var result = _helper.IsList(type) && _helper.GenericArgumentIsNumber(type);

            Assert.True(result);
        }

        [Fact]
        public void IsPrimitiveDecimalList()
        {
            var data = new List<decimal>();
            data.Add(1);
            data.Add(2);
            data.Add(3);
            var type = data.GetType();
            var result = _helper.IsList(type) && _helper.GenericArgumentIsNumber(type);

            Assert.True(result);
        }

        [Fact]
        public void IsPrimitiveDoubleList()
        {
            var data = new List<double>();
            data.Add(1);
            data.Add(2);
            data.Add(3);
            var type = data.GetType();
            var result = _helper.IsList(type) && _helper.GenericArgumentIsNumber(type);

            Assert.True(result);
        }

        [Fact]
        public void IsPrimitiveSingleList()
        {
            var data = new List<Single>();
            data.Add(1);
            data.Add(2);
            data.Add(3);
            var type = data.GetType();
            var result = _helper.IsList(type) && _helper.GenericArgumentIsNumber(type);

            Assert.True(result);
        }

        [Fact]
        public void IsPrimitiveBoolList()
        {
            var data = new List<bool>();
            data.Add(true);
            data.Add(true);
            data.Add(false);
            var type = data.GetType();
            var result = _helper.IsList(type) && _helper.GenericArgumentIsBoolean(type);

            Assert.True(result);
        }

        [Fact]
        public void IsPrimitiveDateList()
        {
            var data = new List<DateTime>();
            data.Add(new DateTime());
            data.Add(new DateTime());
            data.Add(new DateTime());
            var type = data.GetType();
            var result = _helper.IsList(type) && _helper.GenericArgumentIsDate(type);

            Assert.True(result);
        }
    }
}

using System;
using System.Threading.Tasks;
using DynORM.UnitTest.Common;
using DynORM.UnitTest.Models;
using Xunit;

namespace DynORM.UnitTest
{
    public class BasicCrudTest
    {
        [Fact]
        public void InsertTest()
        {
            var repository = RepositoryFactory.Instance.MakeNew<PersonModel>();
            var model = PersonFactory.Instance.MakePerson();
            
            var t = repository.AddConditiion(x => x.Name == "hola").Create(model);
            Task.WaitAll(t);

            Assert.Equal(false, t.IsFaulted);
        }
    }
}

using System;
using System.Threading.Tasks;
using DynORM.Helpers;
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

            repository.Update(new { Email = "" }, null);            
        }
    }
}

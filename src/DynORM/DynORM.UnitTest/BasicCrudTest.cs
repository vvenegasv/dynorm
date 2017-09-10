using System;
using DynORM.UnitTest.Common;
using DynORM.UnitTest.Models;
using Xunit;

namespace DynORM.UnitTest
{
    public class BasicCrudTest
    {
        [Fact]
        public void Test1()
        {
            var repository = RepositoryFactory.Instance.MakeNew<PersonModel>();
            var model = PersonFactory.Instance.MakePerson();
            repository.Create(model);
        }
    }
}

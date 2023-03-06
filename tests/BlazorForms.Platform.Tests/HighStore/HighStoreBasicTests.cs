using BlazorForms.Shared.Extensions;
using BlazorForms.Storage.InMemory;
using BlazorForms.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Platform.Tests.HighStore
{
    public class HighStoreBasicTests
    {
        IHighStore _db;
        public HighStoreBasicTests()
        {
            _db = new InMemoryHighStore();
            var p = new Person { FirstName = "Oleg", LastName = "Ivanov", BirthDate = new DateTime(1990, 10, 2) };
            _db.UpsertAsync(p);
            var p2 = new Person { FirstName = "Ivan", LastName = "Olegov", BirthDate = new DateTime(1970, 10, 2) };
            _db.UpsertAsync(p2);
            var u = new User { Id = 1, Login = "a@a.com", PersonId = p.Id, Person = p };
            _db.UpsertAsync(u);
        }

        [Fact]
        public async Task GetListTest()
        {
            var data = await _db.GetQuery<Person>().ToListAsync();
            Assert.NotEqual(0, data.Count());
        }

        [Fact]
        public async Task GetListOptionsTest()
        {
            var options = new QueryOptions { AllowPagination = true, PageSize = 1 };
            var data = await _db.GetQuery<Person>().ToListAsync(options);
            Assert.NotEqual(1, data.Count());
        }

        [Fact]
        public async Task GetByIdTest()
        {
            var user = await _db.GetQuery<User>().FirstOrDefaultAsync();
            Assert.NotNull(user);
            Assert.Null(user.Person);
        }

        [Fact]
        public async Task GetByIdIncludeTest()
        {
            var q = _db.GetByIdQuery<User>(1).Include(m => m.Person);
            var user = await q.FirstOrDefaultAsync();
            Assert.NotNull(user);
            Assert.NotNull(user.Person);
        }

        [Fact]
        public async Task GetWhereIncludeTest()
        {
            var c = new Company { Name = "Neo1" };
            await _db.UpsertAsync(c);
            var l = new PersonCompanyLink { CompanyId = c.Id, PersonId = 1 };
            await _db.UpsertAsync(l);

            var q = _db.GetQuery<PersonCompanyLink>().Include(m => m.RefPersonLink).Where(m => m.CompanyId == c.Id);
            var data = await q.ToListAsync();
            Assert.NotEmpty(data);
            Assert.Equal(1, data[0].RefPersonLink.Count());
        }
    }
}

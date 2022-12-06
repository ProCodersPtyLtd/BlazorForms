using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Platform.Integration.Tests.TestApi
{
    class TestApiApplication : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            //var root = new InMemoryDatabaseRoot();

            builder.ConfigureServices(services =>
            {
                //services.RemoveAll(typeof(DbContextOptions<NotesDbContext>));
                //services.AddDbContext<NotesDbContext>(options =>
                //    options.UseInMemoryDatabase("Testing", root));
            });

            return base.CreateHost(builder);
        }
    }
}

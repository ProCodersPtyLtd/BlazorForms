//using LinqToDB.Configuration;
//using Microsoft.Extensions.Configuration;
//using Newtonsoft.Json;
//using BlazorForms.Platform.Shared.Exceptions;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace BlazorForms.Platform.Crm.Database.Connection
//{
//    public class CrmSettings : ILinqToDBSettings
//    {
//        public IEnumerable<IDataProviderSettings> DataProviders => Enumerable.Empty<IDataProviderSettings>();

//        public string DefaultConfiguration => "SqlServer";
//        public string DefaultDataProvider => "SqlServer";

//        public IEnumerable<IConnectionStringSettings> ConnectionStrings
//        {
//            get
//            {
//                var config = new ConfigurationBuilder()
//                .AddJsonFile("appsettings.json")
//                .Build();

//                var section = config.GetSection("Linq2Db");

//                if(section == null || section["ConnectionString"] == null)
//                {
//                    throw new ConfigurationException("Section Linq2Db.ConnectionString not found");
//                }

//                yield return
//                    new ConnectionStringSettings
//                    {
//                        Name = "SqlServer",
//                        ProviderName = "SqlServer",
//                        ConnectionString = section["ConnectionString"]
//                    };
//            }
//        }
//    }
//}

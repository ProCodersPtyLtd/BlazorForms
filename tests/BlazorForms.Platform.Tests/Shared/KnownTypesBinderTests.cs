//using Newtonsoft.Json;
//using BlazorForms.Shared;
//using BlazorForms.Platform;
//using BlazorForms.Platform.ProcessFlow.Dto;
//using Xunit;
//using BlazorForms.Platform.Tests.Shared.Models;
//using BlazorForms.Platform.Settings;
//using Microsoft.Extensions.DependencyInjection;
//using BlazorForms.Platform.Shared.ApplicationParts;
//using BlazorForms.Platform.Shared.Interfaces;
//using System.Linq;
//using BlazorForms.Framework.UnitTests.Bases;

//namespace BlazorForms.Platform.Tests.Shared
//{
//    public class KnownTypesBinderTests
//    {
//        protected readonly IProxyScopeConfiguration _proxyScopeConfiguration;
//        protected readonly IKnownTypesBinder _knownTypesBinder;

//        public KnownTypesBinderTests()
//        {
//            AppPartsRegistrationHelper.RegisterPlatformParts(ref _proxyScopeConfiguration, ref _knownTypesBinder);
//        }

//        [Fact]
//        public void DeserializationGenericCollectionsTest()
//        {
//            var model = new Model5 { Client = new ClientModel { FirstName = "Cook" } };
//            model.Client.Emails = new System.Collections.ObjectModel.Collection<EmailModel>(new EmailModel[] { new EmailModel { EmailAddress = "a@a.com" } });
//            var json = JsonConvert.SerializeObject(model, _knownTypesBinder.JsonSerializerSettings);
//            var restoredModel = JsonConvert.DeserializeObject(json, _knownTypesBinder.JsonSerializerSettings) as Model5;
//        }

//        [Fact]
//        public void DeserializationArrayTest()
//        {
//            var model = new FormBusinessRulesDto { DisplayProperties = new FieldDisplayDetails[] { new FieldDisplayDetails { Caption = "kk" } }  };
//            var json = JsonConvert.SerializeObject(model, _knownTypesBinder.JsonSerializerSettings);
//            var restoredModel = JsonConvert.DeserializeObject(json, _knownTypesBinder.JsonSerializerSettings) as FormBusinessRulesDto;
//        }

//        [Fact]
//        public void JsonArraysTest()
//        {
//            string[,] arr = new string[,] { { "c1r1", "c2r1", "c3r1" }, { "c1r2", "c2r2", "c3r2" } };
//            var json = JsonConvert.SerializeObject(arr, _knownTypesBinder.JsonSerializerSettings);
//        }
//    }

//    public class Model5
//    {
//        public virtual Application.DemoInsurance.LowCode.Models.ClientModel Client { get; set; }
//    }
//}

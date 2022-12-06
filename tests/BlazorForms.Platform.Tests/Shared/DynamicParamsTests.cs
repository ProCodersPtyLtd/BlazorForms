//using Newtonsoft.Json;
//using BlazorForms.Platform.Tests.Models;
//using BlazorForms.Flows.Definitions;
//using BlazorForms.Shared;
//using BlazorForms.UnitTests.Bases;
//using BlazorForms.Platform;
//using BlazorForms.Platform.Shared.Interfaces;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using Xunit;

//namespace BlazorForms.Platform.Tests.Shared
//{
//    public class DynamicParamsTests
//    {
//        protected readonly IProxyScopeConfiguration _proxyScopeConfiguration;
//        protected readonly IKnownTypesBinder _knownTypesBinder;

//        public DynamicParamsTests()
//        {
//            AppPartsRegistrationHelper.RegisterPlatformParts(ref _proxyScopeConfiguration, ref _knownTypesBinder);
//        }

//        [Fact]
//        public void DynamicParamsJsonSerializationTest()
//        {
//            var model = new DynamicTestModel();
//            model.FirstName = "Papandreus";
//            (model.Bag as dynamic).Count = 10;

//            var json = JsonConvert.SerializeObject(model, _knownTypesBinder.JsonSerializerSettings);
//            var restoredModel = JsonConvert.DeserializeObject(json, _knownTypesBinder.JsonSerializerSettings) as DynamicTestModel;

//            Assert.Equal("Papandreus", restoredModel.FirstName);
//            Assert.Equal(10, (restoredModel.Bag as dynamic).Count);
//        }
//    }

   
//}

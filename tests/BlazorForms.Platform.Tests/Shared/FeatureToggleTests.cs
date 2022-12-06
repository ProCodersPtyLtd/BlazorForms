//using BlazorForms.Platform.Tests.Shared.Definitions;
//using Xunit;
//using BlazorForms.Platform.Config;

//namespace BlazorForms.Platform.Tests.Shared
//{
//    public class FeatureToggleTests
//    {
//        [Fact]
//        public void DefaultSuppliedReadTest()
//        {
//            Assert.True(Features.MyAwesomeFeature.Enabled);
//        }

//        [Fact]
//        public void DefaultNotSuppliedReadTest()
//        {
//            Assert.False(Features.MyNotSuppliedFeature.Enabled);
//        }

//        [Fact]
//        public void SitEnvironmentFeatureToggleTest()
//        {
//            Assert.True(Features2.MyAwesomeFeature.Enabled);
//            Assert.False(Features2.MyAwesomeFeature2.Enabled);
//            Assert.False(Features2.MyAwesomeFeature3.Enabled);
//        }

//        public static class Features
//        {
//            public static DefaultFeatureToggle MyAwesomeFeature { get; set; } = new MyAwesome();
//            public static DefaultFeatureToggle MyNotSuppliedFeature { get; set; } = new NotSupplied();

//            private class MyAwesome : DefaultFeatureToggle
//            {
//            }

//            private class NotSupplied : DefaultFeatureToggle
//            {
//            }
//        }

//        public static class Features2
//        {
//            public static EnvironmentFeatureToggle MyAwesomeFeature { get; set; } = new MyAwesome();
//            public static EnvironmentFeatureToggle MyAwesomeFeature2 { get; set; } = new MyAwesome2();
//            public static EnvironmentFeatureToggle MyAwesomeFeature3 { get; set; } = new MyAwesome3();

//            private class MyAwesome : EnvironmentFeatureToggle
//            {
//            }

//            private class MyAwesome2 : EnvironmentFeatureToggle
//            {
//            }
//            private class MyAwesome3 : EnvironmentFeatureToggle
//            {
//            }
//        }
//    }

//}

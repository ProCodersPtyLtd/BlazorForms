using BlazorForms.Platform.Config;

namespace BlazorForms.Platform.Settings
{
    public static class PlatformFeatures
    {
        public static EnvironmentFeatureToggle NoCodeStoreEditorFeature { get; set; } = new NoCodeItemStoreEditor();
        public static EnvironmentFeatureToggle NoCodeQueryEditorFeature { get; set; } = new NoCodeQueryEditor();
        public static EnvironmentFeatureToggle NoCodeFormEditorFeature { get; set; } = new NoCodeFormEditor();
        public static EnvironmentFeatureToggle NoCodeFlowEditorFeature { get; set; } = new NoCodeFlowEditor();
        public static EnvironmentFeatureToggle FieldFiltersFeature { get; set; } = new FieldFilters();
        public static EnvironmentFeatureToggle RegisterFeature { get; set; } = new RegisterFeatureToggle();
        public static EnvironmentFeatureToggle FieldSortingFeature { get; set; } = new FieldSorting();

        private class NoCodeItemStoreEditor : EnvironmentFeatureToggle
        {
        }

        private class NoCodeQueryEditor : EnvironmentFeatureToggle
        {
        }

        private class NoCodeFormEditor : EnvironmentFeatureToggle
        {
        }

        private class NoCodeFlowEditor : EnvironmentFeatureToggle
        {
        }
        private class FieldFilters : EnvironmentFeatureToggle
        {
        }
        private class RegisterFeatureToggle : EnvironmentFeatureToggle
        {
        }
        private class FieldSorting : EnvironmentFeatureToggle
        {
        }
    }
}

using BlazorForms.Platform.Config;

namespace BlazorForms.Platform.BackgroundTasks.HostedService
{
    public class HostedFeaturesToggle
    {
        public static EnvironmentFeatureToggle DisableBackgroundServiceFeature { get; set; } = new DisableBackgroundService();

        private class DisableBackgroundService : EnvironmentFeatureToggle
        {
        }
    }
}

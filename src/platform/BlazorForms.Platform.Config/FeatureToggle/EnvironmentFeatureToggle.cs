using FeatureToggle;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlazorForms.Platform.Config
{
    public class EnvironmentFeatureToggle : SimpleFeatureToggle
    {
        static readonly string AllowFeatures;
        static readonly List<string> AllowFeaturesList;

        static EnvironmentFeatureToggle()
        {
            // LogStreamer and AppInsight dependency moved to Application level from framework
            // LogStreamer _logStreamer = new LogStreamer(new TelemetryClient());
            // read config
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var section = config.GetSection("EnvironmentFeatures");

            if (section == null || section["Environment"] == null)
            {
                // _logStreamer.TrackException(new Exception("Section EnvironmentFeatures.Environment not found"));
                throw new Exception("Section EnvironmentFeatures.Environment not found");
            }

            var allowSection = $"{section["Environment"]}_AllowFeatures";

            if (allowSection == null)
            {
                // _logStreamer.TrackException(new Exception($"Section EnvironmentFeatures.{allowSection} not found"));
                throw new Exception($"Section EnvironmentFeatures.{allowSection} not found");
            }

            AllowFeatures = section[allowSection];
            AllowFeaturesList = AllowFeatures.Split(new char[] { ',', ';' }).ToList().Select(f => f.Trim()).ToList();
        }

        public virtual bool Enabled
        {
            get
            {
                try
                {
                    if (AllowFeatures.Trim() == "*")
                    {
                        return this.FeatureEnabled;
                    }

                    return this.FeatureEnabled && AllowFeaturesList.Contains(this.GetType().Name);
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
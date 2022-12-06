using FeatureToggle;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazorForms.Platform.Config
{
    // Simple one - to use without different features in different environments
    public class DefaultFeatureToggle : SimpleFeatureToggle
    {
        public virtual bool Enabled
        {
            get
            {
                try
                {
                    return new DefaultFeatureToggle().FeatureEnabled && this.FeatureEnabled;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}

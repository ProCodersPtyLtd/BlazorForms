using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms
{
    public interface ILogStreamer
    {
        void TrackMetric(string name, long time);
        void TrackMetric(string name, long time, string operationName);
        void TrackException(Exception exc);
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms
{
    public class MockLogStreamer : ILogStreamer
    {
        public MockLogStreamer()
        {
        }

        public void TrackMetric(string name, long time)
        {
        }

        public void TrackMetric(string name, long time, string operationName)
        {
        }

        public void TrackException(Exception exc)
        {
        }
    }
}

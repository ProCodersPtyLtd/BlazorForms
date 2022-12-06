using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace BlazorForms
{
    public class WatchTracer : IDisposable
    {
        readonly Stopwatch _sw;
        readonly string _name;
        private string _operationName;
        private static object _lock = new object();
        private ILogger _logger;
        private ILogStreamer _logStreamer;

        public WatchTracer(ILogger logger, string name, ILogStreamer logStreamer, string operationName = null)
        {
            _logStreamer = logStreamer;
            _logger = logger;
            _name = name;
            _operationName = operationName;
            _sw = new Stopwatch();
            _sw.Start();
        }

        public void Dispose()
        {
            _sw.Stop();
            _logger.LogInformation($"--> [{_name}] takes {_sw.ElapsedMilliseconds}ms");

            if(_operationName == null)
            {
                _logStreamer.TrackMetric(_name, _sw.ElapsedMilliseconds, "Load data");
            }
            else
            {
                _logStreamer.TrackMetric(_name, _sw.ElapsedMilliseconds, _operationName);
            }
            
        }        
    }
}

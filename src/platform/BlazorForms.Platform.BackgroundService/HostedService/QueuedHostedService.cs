using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorForms.Platform.BackgroundTasks.HostedService
{
    public class QueuedHostedService : BackgroundService
    {
        private readonly ILogger _logger;

        public QueuedHostedService(IBackgroundTaskQueue taskQueue, ILoggerFactory loggerFactory)
        {
            TaskQueue = taskQueue;
            _logger = loggerFactory.CreateLogger<QueuedHostedService>();
        }

        public IBackgroundTaskQueue TaskQueue { get; }

        protected async override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Queued Hosted Service is starting.");
            if (HostedFeaturesToggle.DisableBackgroundServiceFeature.Enabled)

            {
                _logger.LogInformation("Queued Hosted Service is *DISABLED*.");
            }
            else
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var workItem = await TaskQueue.DequeueAsync(cancellationToken);

                    try
                    {
                        _logger.LogInformation($"Executing {nameof(workItem)}.");

                        await workItem(cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error occurred executing {WorkItem}.", nameof(workItem));
                    }
                }
            }

            _logger.LogInformation("Queued Hosted Service is stopping.");
        }
    }
}

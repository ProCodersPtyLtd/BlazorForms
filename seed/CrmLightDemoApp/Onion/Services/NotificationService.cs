using CrmLightDemoApp.Onion.Services.Abstractions;

namespace CrmLightDemoApp.Onion.Services
{
    public class NotificationService : INotificationService
    {
        public event EventHandler<MessageEventArgs>? OnMessage;
        public event AsyncEventHandler<MessageEventArgs>? OnMessageAsync;

        public async Task PostMessageAsync(MessageEventArgs args)
        {
            if (OnMessageAsync != null)
            {
                await OnMessageAsync(this, args);
            }
        }

		public void SendMessage(MessageEventArgs args)
		{
			if (OnMessage != null)
			{
				OnMessage(this, args);
			}
		}
	}
}

namespace CrmLightDemoApp.Onion.Services.Abstractions
{
    public interface INotificationService
    {
        event EventHandler<MessageEventArgs> OnMessage;
        event AsyncEventHandler<MessageEventArgs> OnMessageAsync;
        Task PostMessageAsync(MessageEventArgs args); 
        void SendMessage(MessageEventArgs args); 
    }

    public delegate Task AsyncEventHandler<T>(object sender, MessageEventArgs T);

    public class MessageEventArgs
    {
        public string Message { get; set; }
        public MessageEventType Type { get; set; }
    }

    public enum MessageEventType
    {
        TenantAccount,
    }
}

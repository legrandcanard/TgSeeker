
using TdLib;
using TgSeeker.Persistent.Entities;
using TgSeeker.Persistent.Repositiories;
using TgSeeker.Util;
using static TdLib.TdApi;

namespace TgSeeker.EventHandlers.Messages
{
    public abstract class TgsMessageEventHandler
    {
        protected TgsEventHandlerOptions Options { get; private set; }
        protected TdClient Client { get; private set; }
        protected IMessagesRepository MessagesRepository { get; private set; }

        public TgsMessageEventHandler(TgsEventHandlerOptions options, TdClient client, IMessagesRepository messagesRepository)
        {
            Options = options;
            Client = client;
            MessagesRepository = messagesRepository;
        }

        public virtual Task HandleMessageReceivedAsync(TdLib.TdApi.Message message) { return Task.CompletedTask; }
        public abstract Task<Message> HandleMessageDeletedAsync(TgsMessage tgsMessage);
        public virtual Task HandleMessageSendSuccessAsync(TgsMessage tgsMessage) { return Task.CompletedTask; }
        public virtual Task HandleMessageSendFailAsync(TgsMessage tgsMessage) { return Task.CompletedTask; }
    }
}

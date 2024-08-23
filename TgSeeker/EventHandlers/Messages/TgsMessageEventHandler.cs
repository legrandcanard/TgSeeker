
using TdLib;
using TgSeeker.Persistent.Repositiories;
using TgSeeker.Util;

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
    }
}

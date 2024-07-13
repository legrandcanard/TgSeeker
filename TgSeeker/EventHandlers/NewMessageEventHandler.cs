using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TdLib;
using TgSeeker.EventHandlers.Util;
using TgSeeker.Persistent.Repositiories;
using static TdLib.TdApi;
using static TdLib.TdApi.Update;

namespace TgSeeker.EventHandlers
{
    public class NewMessageEventHandler : TgsEventHandler
    {
        protected readonly TgsEventHandlerOptions options;
        protected readonly IMessagesRepository messagesRepository;

        public NewMessageEventHandler(TgsEventHandlerOptions options, TdClient client, IMessagesRepository messagesRepository) : base(client)
        {
            this.options = options;
            this.messagesRepository = messagesRepository;
        }

        public async Task HandleAsync(UpdateNewMessage updateNewMessage)
        {
            var message = updateNewMessage.Message;

            if (options.CurrentUser.Id == message.ChatId)
                return;

            if (message.IsChannelPost || message.IsTopicMessage || message.ChatId < 0)
                return;

            if (message.Content is MessageContent.MessageText textMessage)
            {
                await messagesRepository.CreateMessage(new Persistent.Entities.Message
                {
                    Id = message.Id,
                    ChatId = message.ChatId,
                    Text = textMessage.Text.Text
                });
            }
            else
            {
                // Implement other message types
            }
        }
    }
}

using TdLib;
using TgSeeker.EventHandlers.Exceptions;
using TgSeeker.Persistent.Entities;
using TgSeeker.Persistent.Repositiories;
using TgSeeker.Util;
using static TdLib.TdApi;

namespace TgSeeker.EventHandlers.Messages
{
    internal class TextMessageEventHandler(TgsEventHandlerOptions options, TdClient client, IMessagesRepository messagesRepository) 
        : TgsMessageEventHandler(options, client, messagesRepository)
    {
        public override async Task HandleMessageReceivedAsync(TdApi.Message message)
        {
            if (message.Content is not MessageContent.MessageText textMessage)
                throw new WrongMessageTypeException();

            DateTime createdDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            createdDate = createdDate.AddSeconds(message.Date).ToUniversalTime();

            await MessagesRepository.CreateMessageAsync(new TgsTextMessage
            {
                Id = message.Id,
                ChatId = message.ChatId,
                Text = textMessage.Text.Text,
                CreateDate = createdDate,
            });
        }

        public override async Task<Message> HandleMessageDeletedAsync(TgsMessage tgsMessage)
        {
            var textMessage = tgsMessage as TgsTextMessage ?? throw new WrongMessageTypeException();

            var fromUser = await Client.GetUserAsync(textMessage.ChatId);

            var message = await Client.SendMessageAsync(Options.CurrentUser.Id, inputMessageContent: new TdApi.InputMessageContent.InputMessageText
            {
                Text = new TdApi.FormattedText
                {
                    Text = $"{TgsTextHelper.GetMessageDeletedTitle(fromUser)}:\n{textMessage.Text}"
                }
            });
            await MessagesRepository.DeleteMessageAsync(textMessage.Id);

            return message;
        }

        public override async Task HandleMessageSendSuccessAsync(TgsMessage tgsMessage)
        {
            await MessagesRepository.DeleteMessageAsync(tgsMessage.Id);
        }
    }
}

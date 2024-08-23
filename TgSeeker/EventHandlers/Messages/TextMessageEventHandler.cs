using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TdLib;
using TgSeeker.EventHandlers.Util;
using TgSeeker.Persistent.Entities;
using TgSeeker.Persistent.Repositiories;
using static TdLib.TdApi.Update;
using static TdLib.TdApi;

namespace TgSeeker.EventHandlers.Messages
{
    internal class TextMessageEventHandler : TgsMessageEventHandler
    {
        public TextMessageEventHandler(TgsEventHandlerOptions options, TdClient client, IMessagesRepository messagesRepository) : base(options, client, messagesRepository)
        {
        }

        public async Task HandleCreateAsync(TdApi.Message message)
        {
            if (message.Content is not MessageContent.MessageText textMessage)
                throw new ArgumentException("Wrong message type.");

            await MessagesRepository.CreateMessage(new TgsTextMessage
            {
                Id = message.Id,
                ChatId = message.ChatId,
                Text = textMessage.Text.Text
            });
        }

        public async Task HandleDeleteAsync(TdApi.Update.UpdateDeleteMessages updateDeleteMessages)
        {
            if (updateDeleteMessages.FromCache)
                return;
            if (updateDeleteMessages.ChatId == Options.CurrentUser.Id)
                return;

            var chats = await Client.GetChatsAsync(limit: 10);
            var user = await Client.GetUserAsync(updateDeleteMessages.ChatId);

            var cachedMessages = await MessagesRepository.GetMessagesAsync(updateDeleteMessages.ChatId, updateDeleteMessages.MessageIds);

            if (cachedMessages.Length != updateDeleteMessages.MessageIds.Length)
            {
                await Client.SendMessageAsync(Options.CurrentUser.Id, inputMessageContent: new TdApi.InputMessageContent.InputMessageText
                {
                    Text = new TdApi.FormattedText
                    {
                        Text = $"✉️🔥 {FormatUserDisplayName(user)}\nУдалено {updateDeleteMessages.MessageIds.Length - cachedMessages.Length} соообщений. Кеш не найден.\n(user id: {user.Id})\n<via TgSeeker/>"
                    }
                });
            }

            foreach (TgsTextMessage cachedMessage in cachedMessages)
            {
                await Client.SendMessageAsync(Options.CurrentUser.Id, inputMessageContent: new TdApi.InputMessageContent.InputMessageText
                {
                    Text = new TdApi.FormattedText
                    {
                        Text = $"✉️🔥 {FormatUserDisplayName(user)}:\n{cachedMessage.Text}\n\n(user id: {user.Id})\n<via TgSeeker/>"
                    }
                });
                await MessagesRepository.DeleteMessage(cachedMessage.Id);
            }
        }

        private string FormatUserDisplayName(TdApi.User user)
        {
            string name = string.Join(' ', user.FirstName, user.LastName);
            if (user.Usernames != null && user.Usernames.ActiveUsernames.Length > 0)
            {
                string activeUsername = user.Usernames.ActiveUsernames[0];
                name += " @" + activeUsername;
            }
            return name;
        }
    }
}

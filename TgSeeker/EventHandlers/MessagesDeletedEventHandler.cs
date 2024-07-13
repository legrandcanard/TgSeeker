using TdLib;
using TgSeeker.EventHandlers.Util;
using TgSeeker.Persistent.Repositiories;
using static TdLib.TdApi;
using static TdLib.TdApi.Update;

namespace TgSeeker.EventHandlers
{
    public class MessagesDeletedEventHandler : TgsEventHandler
    {
        protected readonly IMessagesRepository messagesRepository;
        protected readonly TgsEventHandlerOptions options;

        public MessagesDeletedEventHandler(TgsEventHandlerOptions options, TdClient client, IMessagesRepository messagesRepository) : base(client)
        {
            this.options = options;
            this.messagesRepository = messagesRepository;
        }

        public async Task HandleAsync(UpdateDeleteMessages updateDeleteMessages)
        {
            if (updateDeleteMessages.FromCache)
                return;
            if (updateDeleteMessages.ChatId == options.CurrentUser.Id)
                return;

            var chats = await Client.GetChatsAsync(limit: 10);
            var user = await Client.GetUserAsync(updateDeleteMessages.ChatId);

            var cachedMessages = await messagesRepository.GetMessagesAsync(updateDeleteMessages.ChatId, updateDeleteMessages.MessageIds);

            if (cachedMessages.Length != updateDeleteMessages.MessageIds.Length)
            {
                await Client.SendMessageAsync(options.CurrentUser.Id, inputMessageContent: new TdApi.InputMessageContent.InputMessageText
                {
                    Text = new TdApi.FormattedText
                    {
                        Text = $"✉️🔥 {FormatUserDisplayName(user)}\nУдалено {updateDeleteMessages.MessageIds.Length - cachedMessages.Length} соообщений. Кеш не найден.\n(user id: {user.Id})\n<via TgSeeker/>"
                    }
                });
            }

            foreach (var cachedMessage in cachedMessages)
            {
                await Client.SendMessageAsync(options.CurrentUser.Id, inputMessageContent: new TdApi.InputMessageContent.InputMessageText
                {
                    Text = new TdApi.FormattedText
                    {
                        Text = $"✉️🔥 {FormatUserDisplayName(user)}:\n{cachedMessage.Text}\n\n(user id: {user.Id})\n<via TgSeeker/>"
                    }
                });
                await messagesRepository.DeleteMessage(cachedMessage.Id);
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

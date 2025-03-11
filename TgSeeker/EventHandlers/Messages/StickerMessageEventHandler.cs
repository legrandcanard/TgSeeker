using TdLib;
using TgSeeker.Persistent.Entities;
using TgSeeker.Persistent.Repositiories;
using TgSeeker.Util;
using static TdLib.TdApi;
using static TdLib.TdApi.MessageContent;

namespace TgSeeker.EventHandlers.Messages
{
    internal class StickerMessageEventHandler : TgsMessageEventHandler
    {
        public StickerMessageEventHandler(TgsEventHandlerOptions options, TdClient client, IMessagesRepository messagesRepository)
            : base(options, client, messagesRepository) { }

        public override async Task HandleMessageReceivedAsync(TdApi.Message message)
        {
            var stickerMessage = message.Content as MessageSticker;

            var sendingMessage = await Client.SendMessageAsync(Options.CurrentUser.Id, inputMessageContent: new TdApi.InputMessageContent.InputMessageSticker 
            { 
                Emoji = stickerMessage.Sticker.Emoji,
                Sticker = new TdApi.InputFile.InputFileRemote
                {
                    Id = stickerMessage.Sticker.Sticker_.Remote.Id
                }
            });
        }

        public override Task<TdApi.Message> HandleMessageDeletedAsync(TgsMessage tgsMessage)
        {
            throw new NotImplementedException();
        }

        public override Task RemoveCacheForMessageAsync(TgsMessage tgsMessage)
        {
            throw new NotImplementedException();
        }
    }
}

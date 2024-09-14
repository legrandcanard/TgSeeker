using TgSeeker.Persistent.Entities;
using TgSeeker.Persistent.Repositiories;
using TgSeeker.EventHandlers.Exceptions;
using TgSeeker.Util;
using TdLib;
using static TdLib.TdApi.InputFile;

namespace TgSeeker.EventHandlers.Messages
{
    public class VideoNoteMessageEventHandler(TgsEventHandlerOptions options, TdClient client, IMessagesRepository messagesRepository)
        : TgsMessageEventHandler(options, client, messagesRepository)
    {
        private const string VideoNoteDir = "videoNote";

        public async Task HandleCreateAsync(TdApi.Message message) 
        {
            var videoNoteMsg = message.Content as TdApi.MessageContent.MessageVideoNote ?? throw new WrongMessageTypeException();

            var file = await Client.DownloadFileAsync(videoNoteMsg.VideoNote.Video.Id, priority: 3, limit: int.MaxValue, synchronous: true);

            string localFileId = file.Remote.UniqueId;
            await FileCacheManager.CacheFileAsync(file.Local.Path, VideoNoteDir, localFileId);
                        
            await MessagesRepository.CreateMessageAsync(new TgsVideoNoteMessage
            {
                Id = message.Id,
                ChatId = message.ChatId,
                CreateDate = message.GetCreationDate(),

                // VideoNote
                Duration = videoNoteMsg.VideoNote.Duration,
                Length = videoNoteMsg.VideoNote.Length,
                Waveform = videoNoteMsg.VideoNote.Waveform,
                LocalFileId = localFileId
            });
        }

        public async Task<TdApi.Message> HandleDeleteAsync(TgsVideoNoteMessage videoNoteMessage) 
        {
            var fromUser = await Client.GetUserAsync(videoNoteMessage.ChatId);

            await Client.SendMessageAsync(Options.CurrentUser.Id, inputMessageContent: new TdApi.InputMessageContent.InputMessageText
            {
                Text = new TdApi.FormattedText { Text = TgsTextHelper.GetMessageDeletedTitle(fromUser) }
            });

            return await Client.SendMessageAsync(Options.CurrentUser.Id, inputMessageContent: new TdApi.InputMessageContent.InputMessageVideoNote
            {
                Duration = videoNoteMessage.Duration,
                VideoNote = new InputFileLocal
                {
                    Path = FileCacheManager.GetFullFilePath(VideoNoteDir, videoNoteMessage.LocalFileId),
                }
            });   
        }

        public async Task HandleMessageCopySentCompleteAsync(TdApi.Message message, TgsVideoNoteMessage sourceMessage)
        {
            FileCacheManager.Purge(VideoNoteDir, sourceMessage.LocalFileId);
            await MessagesRepository.DeleteMessageAsync(sourceMessage.Id);
        }
    }
}

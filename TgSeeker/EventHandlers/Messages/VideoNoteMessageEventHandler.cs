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
        private const string VideoNoteThumbnailDir = "videoNoteThumb";

        public async Task HandleCreateAsync(TdApi.Message message) 
        {
            var videoNoteMsg = message.Content as TdApi.MessageContent.MessageVideoNote ?? throw new WrongMessageTypeException();

            var file = await Client.DownloadFileAsync(videoNoteMsg.VideoNote.Video.Id, priority: 3, limit: int.MaxValue, synchronous: true);

            string localFileId = file.Remote.UniqueId;
            await FileCacheManager.CacheFileAsync(file.Local.Path, VideoNoteDir, localFileId);

            string thumbnailLocalFileId = videoNoteMsg.VideoNote.Thumbnail.File.Remote.UniqueId;
            await FileCacheManager.CacheFileAsync(file.Local.Path, VideoNoteThumbnailDir, thumbnailLocalFileId);
            
            await MessagesRepository.CreateMessageAsync(new TgsVideoNoteMessage
            {
                Id = message.Id,
                ChatId = message.ChatId,
                CreateDate = message.GetCreationDate(),

                // VideoNote
                Duration = videoNoteMsg.VideoNote.Duration,
                Length = videoNoteMsg.VideoNote.Length,
                Waveform = videoNoteMsg.VideoNote.Waveform,
                LocalFileId = localFileId,

                //// Minithumbnail
                //MinithumbnailData = videoNoteMsg.VideoNote.Minithumbnail.Data,
                //MinithumbnailHeight = videoNoteMsg.VideoNote.Minithumbnail.Height,
                //MinithumbnailWidth = videoNoteMsg.VideoNote.Minithumbnail.Width,
                
                //// Thumbnail
                //ThumbnailFormat = videoNoteMsg.VideoNote.Thumbnail.Format.DataType,
                //ThumbnailHeight = videoNoteMsg.VideoNote.Thumbnail.Height,
                //ThumbnailWidth = videoNoteMsg.VideoNote.Thumbnail.Width,
                //ThumbnailLocalFileId = thumbnailLocalFileId
            });
        }

        public async Task HandleDeleteAsync(TgsVideoNoteMessage videoNoteMessage) 
        {
            var fromUser = await Client.GetUserAsync(videoNoteMessage.ChatId);

            await Client.SendMessageAsync(Options.CurrentUser.Id, inputMessageContent: new TdApi.InputMessageContent.InputMessageText
            {
                Text = new TdApi.FormattedText { Text = TgsTextHelper.GetMessageDeletedTitle(fromUser) }
            });

            await Client.SendMessageAsync(Options.CurrentUser.Id, inputMessageContent: new TdApi.InputMessageContent.InputMessageVideoNote
            {
                Duration = videoNoteMessage.Duration,
                VideoNote = new InputFileLocal
                {
                    Path = FileCacheManager.GetFullFilePath(VideoNoteDir, videoNoteMessage.LocalFileId),
                }
            });

            await MessagesRepository.DeleteMessageAsync(videoNoteMessage.Id);

            //FileCacheManager.Purge(VideoNoteDir, videoNoteMessage.LocalFileId);
            //FileCacheManager.Purge(VideoNoteThumbnailDir, videoNoteMessage.ThumbnailLocalFileId);
        }
    }
}

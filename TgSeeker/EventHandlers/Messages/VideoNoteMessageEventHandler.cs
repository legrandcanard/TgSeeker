using TgSeeker.Persistent.Entities;
using TgSeeker.Persistent.Repositiories;
using TgSeeker.EventHandlers.Exceptions;
using TgSeeker.Util;
using TdLib;
using static TdLib.TdApi.InputFile;
using static TdLib.TdApi;

namespace TgSeeker.EventHandlers.Messages
{
    public class VideoNoteMessageEventHandler
        : TgsMessageEventHandler
    {
        readonly TgsEventHandlerOptions _options;
        readonly TdClient _client;
        readonly IMessagesRepository _messagesRepository;

        private const string VideoNoteDir = TgsContants.VideoNoteMessageFsCacheDirName;

        public VideoNoteMessageEventHandler(
            TgsEventHandlerOptions options, 
            TdClient client, 
            IMessagesRepository messagesRepository)
            : base(options, client, messagesRepository)
        {
            _options = options;
            _client = client;
            _messagesRepository = messagesRepository;
        }

        public override async Task HandleMessageReceivedAsync(Message message)
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

        public override async Task<Message> HandleMessageDeletedAsync(TgsMessage tgsMessage)
        {
            var videoNoteMessage = tgsMessage as TgsVideoNoteMessage ?? throw new WrongMessageTypeException();

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

        public override async Task HandleMessageSendSuccessAsync(TgsMessage tgsMessage)
        {
            var videoNoteMessage = tgsMessage as TgsVideoNoteMessage ?? throw new WrongMessageTypeException();

            FileCacheManager.Purge(VideoNoteDir, videoNoteMessage.LocalFileId);
            await MessagesRepository.DeleteMessageAsync(videoNoteMessage.Id);
        }
    }
}

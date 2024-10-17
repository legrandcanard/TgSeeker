using TdLib;
using TgSeeker.Persistent.Repositiories;
using TgSeeker.Persistent.Entities;
using static TdLib.TdApi.InputFile;
using TgSeeker.Util;
using TgSeeker.EventHandlers.Exceptions;

namespace TgSeeker.EventHandlers.Messages
{
    public class VoiceMessageEventHandler(TgsEventHandlerOptions options, TdClient client, IMessagesRepository messagesRepository)
        : TgsMessageEventHandler(options, client, messagesRepository)
    {
        public const string VoiceFileExtenion = "ogg";
        private const string VoiceNoteDir = TgsContants.VoiceNoteMessageFsCacheDirName;

        public override async Task HandleMessageReceivedAsync(TdApi.Message message)
        {
            if (message.Content is not TdApi.MessageContent.MessageVoiceNote textMessage)
                throw new WrongMessageTypeException();

            var voiceNoteMsg = message.Content as TdApi.MessageContent.MessageVoiceNote ?? throw new Exception("Wrong message type");

            var file = await Client.DownloadFileAsync(voiceNoteMsg.VoiceNote.Voice.Id, priority: 3, limit: int.MaxValue, synchronous: true);

            string localFileId = file.Remote.UniqueId;
            await FileCacheManager.CacheFileAsync(file.Local.Path, VoiceNoteDir, localFileId);

            await MessagesRepository.CreateMessageAsync(new TgsVoiceMessage
            {
                Id = message.Id,
                ChatId = message.ChatId,
                CreateDate = message.GetCreationDate(),
                LocalFileId = file.Remote.UniqueId,
                Waveform = voiceNoteMsg.VoiceNote.Waveform,
                Duration = voiceNoteMsg.VoiceNote.Duration,
            });
        }

        public override async Task<TdApi.Message> HandleMessageDeletedAsync(TgsMessage tgsMessage)
        {
            var voiceMessage = tgsMessage as TgsVoiceMessage ?? throw new WrongMessageTypeException();

            var fromUser = await Client.GetUserAsync(voiceMessage.ChatId);

            string filePath = Path.Combine(VoiceNoteDir, $"{voiceMessage.LocalFileId}.{VoiceFileExtenion}");

            return await Client.SendMessageAsync(Options.CurrentUser.Id, inputMessageContent: new TdApi.InputMessageContent.InputMessageVoiceNote
            {
                Waveform = voiceMessage.Waveform,
                Duration = voiceMessage.Duration,
                Caption = new TdApi.FormattedText { Text = TgsTextHelper.GetMessageDeletedTitle(fromUser) },
                VoiceNote = new InputFileLocal
                {
                    Path = FileCacheManager.GetFullFilePath(VoiceNoteDir, voiceMessage.LocalFileId),
                }
            });
        }

        public override async Task HandleMessageSendSuccessAsync(TgsMessage tgsMessage)
        {
            var voiceMessage = tgsMessage as TgsVoiceMessage ?? throw new WrongMessageTypeException();

            FileCacheManager.Purge(VoiceNoteDir, voiceMessage.LocalFileId);
            await MessagesRepository.DeleteMessageAsync(voiceMessage.Id);
        }
    }
}

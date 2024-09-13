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
        public static readonly string voiceDirPath = Path.Combine(Directory.GetCurrentDirectory(), "cache\\voice\\");
        public const string VoiceFileExtenion = "ogg";

        public async Task HandleCreateAsync(TdApi.Message message)
        {
            if (message.Content is not TdApi.MessageContent.MessageVoiceNote textMessage)
                throw new WrongMessageTypeException();

            var voiceNoteMsg = message.Content as TdApi.MessageContent.MessageVoiceNote ?? throw new Exception("Wrong message type");

            var file = await Client.DownloadFileAsync(voiceNoteMsg.VoiceNote.Voice.Id, priority: 3, limit: int.MaxValue, synchronous: true);

            Directory.CreateDirectory(voiceDirPath);

            using var newFileFs = File.Create(Path.Combine(voiceDirPath, $"{voiceDirPath + file.Remote.UniqueId}.{VoiceFileExtenion}"));
            using var tdlibFileCacheFs = File.OpenRead(file.Local.Path);
            tdlibFileCacheFs.CopyTo(newFileFs);

            DateTime createdDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            createdDate = createdDate.AddSeconds(message.Date).ToUniversalTime();

            await MessagesRepository.CreateMessageAsync(new TgsVoiceMessage
            {
                Id = message.Id,
                ChatId = message.ChatId,
                CreateDate = createdDate,
                LocalFileId = file.Remote.UniqueId,
                Waveform = voiceNoteMsg.VoiceNote.Waveform,
                Duration = voiceNoteMsg.VoiceNote.Duration,
            });
        }

        public async Task HandleDeleteAsync(TgsVoiceMessage voiceMessage)
        {
            var fromUser = await Client.GetUserAsync(voiceMessage.ChatId);

            string filePath = Path.Combine(voiceDirPath, $"{voiceMessage.LocalFileId}.{VoiceFileExtenion}");

            await Client.SendMessageAsync(Options.CurrentUser.Id, inputMessageContent: new TdApi.InputMessageContent.InputMessageVoiceNote
            {
                Waveform = voiceMessage.Waveform,
                Duration = voiceMessage.Duration,
                Caption = new TdApi.FormattedText { Text = TgsTextHelper.GetMessageDeletedTitle(fromUser) },
                VoiceNote = new InputFileLocal
                {
                    Path = filePath
                }
            });

            await MessagesRepository.DeleteMessageAsync(voiceMessage.Id);

            File.Delete(filePath);
        }
    }
}

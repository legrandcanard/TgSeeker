using TdLib;
using TgSeeker.EventHandlers.Util;
using TgSeeker.Persistent.Repositiories;
using TgSeeker.Persistent.Entities;
using static TdLib.TdApi.InputFile;
using System.Reflection;

namespace TgSeeker.EventHandlers.Messages
{
    public class VoiceMessageEventHandler(TgsEventHandlerOptions options, TdClient client, IMessagesRepository messagesRepository)
        : TgsMessageEventHandler(options, client, messagesRepository)
    {
        public static readonly string voiceDirPath = Path.Combine(Directory.GetCurrentDirectory(), "cache\\voice\\");
        public const string VoiceFileExtenion = "ogg";

        public async Task HandleCreateAsync(TdApi.Message message)
        {
            var voiceNoteMsg = message.Content as TdApi.MessageContent.MessageVoiceNote ?? throw new Exception("Wrong message type");

            var file = await Client.DownloadFileAsync(voiceNoteMsg.VoiceNote.Voice.Id, priority: 3, limit: int.MaxValue, synchronous: true);

            Directory.CreateDirectory(voiceDirPath);

            using var newFileFs = File.Create(Path.Combine(voiceDirPath, $"{voiceDirPath + file.Remote.UniqueId}.{VoiceFileExtenion})"));
            using var tdlibFileCacheFs = File.OpenRead(file.Local.Path);
            tdlibFileCacheFs.CopyTo(newFileFs);

            await MessagesRepository.CreateMessage(new TgsVoiceMessage
            {
                Id = message.Id,
                ChatId = message.ChatId,
                LocalFileId = file.Remote.UniqueId,
                Waveform = voiceNoteMsg.VoiceNote.Waveform,
                Duration = voiceNoteMsg.VoiceNote.Duration,
            });
        }

        public async Task HandleDeleteAsync(TgsVoiceMessage voiceMessage)
        {
            string filePath = Path.Combine(voiceDirPath, $"{voiceMessage.LocalFileId}.{VoiceFileExtenion}");

            await Client.SendMessageAsync(Options.CurrentUser.Id, inputMessageContent: new TdApi.InputMessageContent.InputMessageVoiceNote
            {
                Waveform = voiceMessage.Waveform,
                Duration = voiceMessage.Duration,
                VoiceNote = new InputFileLocal
                {
                    Path = filePath
                }
            });

            await MessagesRepository.DeleteMessage(voiceMessage.Id);

            File.Delete(filePath);
        }
    }
}

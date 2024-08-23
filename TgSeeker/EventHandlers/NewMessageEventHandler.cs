using TdLib;
using TgSeeker.EventHandlers.Util;
using TgSeeker.Persistent.Repositiories;
using static TdLib.TdApi;
using static TdLib.TdApi.Update;

namespace TgSeeker.EventHandlers
{
    public class NewMessageEventHandler : TgsEventHandler
    {
        protected readonly TgsEventHandlerOptions options;
        protected readonly IMessagesRepository messagesRepository;

        public NewMessageEventHandler(TgsEventHandlerOptions options, TdClient client, IMessagesRepository messagesRepository) : base(client)
        {
            this.options = options;
            this.messagesRepository = messagesRepository;
        }

        public async Task HandleAsync(UpdateNewMessage updateNewMessage)
        {
            var message = updateNewMessage.Message;

            if (options.CurrentUser.Id == message.ChatId)
                return;

            if (message.IsChannelPost || message.IsTopicMessage || message.ChatId < 0)
                return;

            if (message.Content is MessageContent.MessageText textMessage)
            {
                await messagesRepository.CreateMessage(new Persistent.Entities.TgsTextMessage
                {
                    Id = message.Id,
                    ChatId = message.ChatId,
                    Text = textMessage.Text.Text
                });
            }
            else if (message.Content is MessageContent.MessageVoiceNote voiceNoteMessage) 
            {
                //try
                //{
                //    await Client.SendMessageAsync(options.CurrentUser.Id, inputMessageContent: new TdApi.InputMessageContent.InputMessageText
                //    {
                //        Text = new TdApi.FormattedText
                //        {
                //            Text = $"✉️🔥Test <via TgSeeker/>"
                //        }
                //    });

                //    var file = await Client.DownloadFileAsync(voiceNoteMessage.VoiceNote.Voice.Id, priority: 3, limit: int.MaxValue, synchronous: true);
                //    var msg = await Client.SendMessageAsync(options.CurrentUser.Id, inputMessageContent: new TdApi.InputMessageContent.InputMessageVoiceNote
                //    {
                //        Waveform = voiceNoteMessage.VoiceNote.Waveform,
                //        Caption = voiceNoteMessage.Caption,
                //        Duration = voiceNoteMessage.VoiceNote.Duration,
                //        Extra = voiceNoteMessage.VoiceNote.Extra,
                //        VoiceNote = new InputFile.InputFileRemote
                //        {
                //            Id = file.Remote.Id
                //        }
                //    });
                //}
                //catch (Exception ex)
                //{
                //}
            }
        }
    }
}

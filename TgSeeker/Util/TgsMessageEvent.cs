using TgSeeker.EventHandlers.Messages;
using TgSeeker.Persistent.Entities;
using TgSeeker.Persistent.Repositiories;
using TdLib;
using static TdLib.TdApi.MessageContent;

namespace TgSeeker.Util
{
    public static class TgsMessageEvent
    {
        public static TgsMessageEventHandler For(TgsMessage message, TgsEventHandlerOptions options, IMessagesRepository messagesRepository, TdClient tdClient)
        {
            return message switch
            {
                TgsTextMessage textMessage => new TextMessageEventHandler(options, tdClient, messagesRepository),
                TgsVoiceMessage voiceMessage => new VoiceMessageEventHandler(options, tdClient, messagesRepository),
                TgsVideoNoteMessage videoNoteMessage => new VideoNoteMessageEventHandler(options, tdClient, messagesRepository),
                _ => throw new ArgumentOutOfRangeException($"Message not supported: {message.GetType()}.")
            };
        }

        public static TgsMessageEventHandler For(TdLib.TdApi.Message message, TgsEventHandlerOptions options, IMessagesRepository messagesRepository, TdClient tdClient)
        {
            return message.Content switch
            {
                MessageText textMessage => new TextMessageEventHandler(options, tdClient, messagesRepository),
                MessageVoiceNote voiceMessage => new VoiceMessageEventHandler(options, tdClient, messagesRepository),
                MessageVideoNote videoNoteMessage => new VideoNoteMessageEventHandler(options, tdClient, messagesRepository),
                _ => throw new ArgumentOutOfRangeException($"Message not supported: {message.GetType()}.")
            };
        }
    }
}

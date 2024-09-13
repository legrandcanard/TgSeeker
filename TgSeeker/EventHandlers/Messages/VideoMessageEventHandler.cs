using TgSeeker.Persistent.Entities;
using TgSeeker.Persistent.Repositiories;
using TgSeeker.EventHandlers.Exceptions;
using TgSeeker.Util;
using TdLib;
using static TdLib.TdApi.InputFile;

namespace TgSeeker.EventHandlers.Messages
{
    public class VideoMessageEventHandler(TgsEventHandlerOptions options, TdClient client, IMessagesRepository messagesRepository)
        : TgsMessageEventHandler(options, client, messagesRepository)
    {
        public async Task HandleCreateAsync(TdApi.Message message) 
        {
            
        }

        public async Task HandleDeleteAsync(TgsVoiceMessage voiceMessage) 
        {
            
        }
    }
}

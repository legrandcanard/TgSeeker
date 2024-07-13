using TdLib;

namespace TgSeeker.EventHandlers
{
    public abstract class TgsEventHandler
    {
        protected TdClient Client { get; private set; }

        public TgsEventHandler(TdClient client)
        {
            Client = client;
        }
    }
}

using TdLib;

namespace TgSeeker.Util
{
    public static class TdMessageExt
    {
        public static DateTime GetCreationDate(this TdApi.Message message)
        {
            DateTime createdDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            createdDate = createdDate.AddSeconds(message.Date).ToUniversalTime();
            return createdDate;
        }
    }
}

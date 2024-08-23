using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TdLib;

namespace TgSeeker.Util
{
    internal static class TgsTextHelper
    {
        public static string FormatUserDisplayName(TdApi.User user)
        {
            string name = string.Join(' ', user.FirstName, user.LastName);
            if (user.Usernames != null && user.Usernames.ActiveUsernames.Length > 0)
            {
                string activeUsername = user.Usernames.ActiveUsernames[0];
                name += " @" + activeUsername;
            }
            name += $"(id: {user.Id})";
            return name;
        }

        public static string GetMessageDeletedTitle(TdApi.User user)
        {
            return $"✉️🔥 [𝘁𝗴𝘀] {TgsLocale.MessageFrom} {FormatUserDisplayName(user)}";
        }
    }
}

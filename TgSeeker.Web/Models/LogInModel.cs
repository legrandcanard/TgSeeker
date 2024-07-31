using System.ComponentModel.DataAnnotations;

namespace TgSeeker.Web.Models
{
    public class LogInModel
    {
        public string Username { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

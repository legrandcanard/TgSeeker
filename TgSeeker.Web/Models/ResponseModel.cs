namespace TgSeeker.Web.Models
{
    public class ResponseModel
    {
        public bool Ok { get; set; } = true;
        public string? Error { get; set; }

        public ResponseModel(bool ok = true, string? error = null)
        {
            Ok = ok;
            Error = error;
        }
    }
}

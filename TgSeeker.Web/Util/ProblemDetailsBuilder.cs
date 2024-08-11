using Microsoft.AspNetCore.Mvc;

namespace TgSeeker.Web.Util
{
    public class ProblemDetailsBuilder
    {
        private readonly ProblemDetails _problemDetails = new ProblemDetails();
        private List<string> _currentControlMessages;
        protected string Name { get; set; } = null!;
        

        public ProblemDetailsBuilder ForControl(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            Name = name;
            _currentControlMessages = new List<string>();

            _problemDetails.Extensions = new Dictionary<string, object?>
            {
                { "errors", new Dictionary<string, object?> {
                    { Name, _currentControlMessages }
                } }
            };
            return this;
        }

        public ProblemDetailsBuilder HasMessage(string message)
        {
            if (Name == null)
                throw new ArgumentNullException(nameof(Name));

            _currentControlMessages.Add(message);

            return this;
        }

        public ProblemDetails Create(params string[] messages)
        {
            if (Name == null)
                throw new ArgumentNullException(nameof(Name));

            return _problemDetails;
        }
    }
}

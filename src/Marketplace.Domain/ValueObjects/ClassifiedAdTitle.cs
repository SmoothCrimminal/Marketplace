using System.Text.RegularExpressions;

namespace Marketplace.Domain.ValueObjects
{
    public sealed record ClassifiedAdTitle
    {
        private readonly string _value;

        internal ClassifiedAdTitle(string value)
        {
            _value = value;
        }

        public static ClassifiedAdTitle FromString(string title)
        {
            CheckValidity(title);
            return new ClassifiedAdTitle(title);
        }

        public static ClassifiedAdTitle FromHtml(string htmlTitle)
        {
            var supportedTagsReplaced = htmlTitle
                .Replace("<i>", "*")
                .Replace("</i>", "*")
                .Replace("<b>", "**")
                .Replace("</b>", "**");

            var value = Regex.Replace(supportedTagsReplaced, "<.*?>", string.Empty);
            CheckValidity(value);

            return new ClassifiedAdTitle(value);
        }

        private static void CheckValidity(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Title cannot be empty", nameof(value));

            if (value.Length > 100)
                throw new ArgumentOutOfRangeException("Title cannot be longer than 100 characters", nameof(value));
        }

        public static implicit operator string(ClassifiedAdTitle self) => self._value;
    }
}

namespace Marketplace.Domain.ValueObjects
{
    public sealed record ClassifiedAdText
    {
        private readonly string _value;

        internal ClassifiedAdText(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Text cannot be empty", nameof(value));

            _value = value;
        }

        public static ClassifiedAdText FromString(string text) => new ClassifiedAdText(text);

        public static implicit operator string(ClassifiedAdText self) => self._value;
    }
}

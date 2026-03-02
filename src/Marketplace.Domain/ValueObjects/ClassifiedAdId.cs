namespace Marketplace.Domain.ValueObjects
{
    public sealed record ClassifiedAdId
    {
        private readonly Guid _value;

        public ClassifiedAdId(Guid value)
        {
            if (value == Guid.Empty || value == default)
                throw new ArgumentNullException("Classified ad id cannot be empty", nameof(value));

            _value = value;
        }

        public static implicit operator Guid(ClassifiedAdId self) => self._value;
    }
}

namespace Marketplace.Domain.Exceptions
{
    public class InvalidEntityStateException : Exception
    {
        public InvalidEntityStateException(object entity, string message) : base($"Entity change {entity.GetType().Name} was rejected, {message}")
        {
            
        }
    }
}

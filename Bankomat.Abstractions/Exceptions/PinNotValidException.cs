using System;
using System.Runtime.Serialization;

namespace Bankomat.Abstractions.Exceptions
{
    [Serializable]
    public class PinNotValidException : Exception
    {
        public PinNotValidException()
        {
        }

        public PinNotValidException(string message) : base(message)
        {
        }

        public PinNotValidException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PinNotValidException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
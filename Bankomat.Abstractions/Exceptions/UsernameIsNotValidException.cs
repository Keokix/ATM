using System;
using System.Runtime.Serialization;

namespace Bankomat.Abstractions.Exceptions
{
    [Serializable]
    public class UsernameIsNotValidException : Exception
    {
        public UsernameIsNotValidException()
        {
        }

        public UsernameIsNotValidException(string message) : base(message)
        {
        }

        public UsernameIsNotValidException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UsernameIsNotValidException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
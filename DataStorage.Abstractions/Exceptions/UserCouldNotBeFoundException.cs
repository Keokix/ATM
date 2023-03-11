using System;
using System.Runtime.Serialization;

namespace DataStorage.Abstractions.Exceptions
{
    [Serializable]
    public class UserCouldNotBeFoundException : Exception
    {
        public UserCouldNotBeFoundException()
        {
        }

        public UserCouldNotBeFoundException(string message) : base(message)
        {
        }

        public UserCouldNotBeFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UserCouldNotBeFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
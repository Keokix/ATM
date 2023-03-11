using System;
using System.Runtime.Serialization;

namespace Bankomat.Abstractions.Exceptions
{
    [Serializable]
    public class AmountToDepositCantBeNegativeException : Exception
    {
        public AmountToDepositCantBeNegativeException()
        {
        }

        public AmountToDepositCantBeNegativeException(string message) : base(message)
        {
        }

        public AmountToDepositCantBeNegativeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AmountToDepositCantBeNegativeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
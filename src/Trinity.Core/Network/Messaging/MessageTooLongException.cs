using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Network.Messaging
{
    /// <summary>
    /// This exception indicates that the length of the response exceeds the maximum (Int32.MaxValue)
    /// of a single Trinity Message.
    /// When the exception is thrown in a message handler, a E_MSG_OVERFLOW will be transfered to the client side.
    /// The client side should then interpret the error code and propagate the <c>MessageTooLongException</c>.
    /// </summary>
    [Serializable]
    public class MessageTooLongException : Exception
    {
        /// <summary>
        /// Initialize a new MessageTooLongException instance.
        /// </summary>
        public MessageTooLongException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MessageTooLongException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public MessageTooLongException(string message)
            : base(message)
        {
        }

        /// <summary>Initializes a new instance of the MessageTooLongException class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
        /// <param name="message">The error message that explains the reason for the exception. </param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception. </param>
        public MessageTooLongException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

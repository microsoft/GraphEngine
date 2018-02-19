// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trinity.Storage
{
    /// <summary>
    /// The exception that is thrown when a cell is not found.
    /// </summary>
    [Serializable]
    public class CellNotFoundException : Exception
    {
        /// <summary>
        /// Initialize a new CellNotFoundException instance.
        /// </summary>
        public CellNotFoundException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the CellNotFoundException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public CellNotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>Initializes a new instance of the CellNotFoundException class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
        /// <param name="message">The error message that explains the reason for the exception. </param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception. </param>
        public CellNotFoundException(string message,
      Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// The exception that is thrown when the specified cell type does not match the loaded cell type.
    /// </summary>
    [Serializable]
    public class CellTypeNotMatchException : Exception
    {
        /// <summary>
        /// Initialize a new CellTypeNotMatchException instance.
        /// </summary>
        public CellTypeNotMatchException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the CellTypeNotMatchException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public CellTypeNotMatchException(string message)
            : base(message)
        {
        }

        /// <summary>Initializes a new instance of the CellTypeNotMatchException class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
        /// <param name="message">The error message that explains the reason for the exception. </param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception. </param>
        public CellTypeNotMatchException(string message,
      Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// The exception that is thrown when attempting to apply operations on incompatible data types.
    /// </summary>
    [Serializable]
    public class DataTypeIncompatibleException : Exception
    {
        /// <summary>
        /// Initialize a new DataTypeIncompatibleException instance.
        /// </summary>
        public DataTypeIncompatibleException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the DataTypeIncompatibleException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DataTypeIncompatibleException(string message)
            : base(message)
        {
        }

        /// <summary>Initializes a new instance of the DataTypeIncompatibleException class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
        /// <param name="message">The error message that explains the reason for the exception. </param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception. </param>
        public DataTypeIncompatibleException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
    
    /// <summary>
    /// The exception that is thrown when attempting to perform invalid resize operations on <see cref="IAccessor"/>.
    /// </summary>
    [Serializable]
    public class AccessorResizeException : Exception
    {
        /// <summary>
        /// Initialize a new AccessorResizeException instance.
        /// </summary>
        public AccessorResizeException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the AccessorResizeException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public AccessorResizeException(string message)
            : base(message)
        {
        }

        /// <summary>Initializes a new instance of the AccessorResizeException class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
        /// <param name="message">The error message that explains the reason for the exception. </param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception. </param>
        public AccessorResizeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

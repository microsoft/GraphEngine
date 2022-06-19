// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trinity.Network.Http
{
    /// <summary>
    /// Represents a bad request recognized by a Trinity Http handler.
    /// When this handler is thrown, the error message is passed
    /// to the client with code 400. 
    /// Other types of exceptions thrown by a handler results in 
    /// an error message "Internal server error" passed to the 
    /// client with code 500.
    /// </summary>
    public class BadRequestException : Exception
    {
        /// <summary>
        /// Represents the error code.
        /// </summary>
        public string Code;
        /// <summary>
        /// Initializes a new instance of the Trinity.Network.Http.BadRequestException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="code">The error code. The default value is "BadArgument".</param>
        public BadRequestException(string message, string code = "BadArgument") : base(message)
        {
            Code = code;
        }
    }
}

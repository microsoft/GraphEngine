// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanoutSearch
{
    public class FanoutSearchQueryException : Exception
    {
        public FanoutSearchQueryException(string message) : base(message) { }
        public FanoutSearchQueryException(string message, Exception inner) : base(message, inner) { }
    }
}

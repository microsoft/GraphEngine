// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Storage;

namespace FanoutSearch
{
    public interface IFanoutSearchCellAccessor : ICellAccessor
    {
        T Cast<T>() where T : ICellAccessor;
        bool isOfType(string type_name);
    }
}

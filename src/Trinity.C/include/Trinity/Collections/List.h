// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include <vector>

namespace Trinity
{
	namespace Collections
	{
		template<class _Ty, class _Alloc = std::allocator<_Ty> >
		using List = std::vector < _Ty, _Alloc > ;
	}
}
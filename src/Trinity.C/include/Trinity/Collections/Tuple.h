// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include <tuple>

namespace Trinity
{
	namespace Collections
	{
		template <typename ...T> using Tuple = std::tuple < T... > ;
	}
}